//
//  EventRepository.m
//  RudderSDKCore
//
//  Created by Arnab Pal on 17/10/19.
//  Copyright © 2019 Rudderlabs. All rights reserved.
//

#import "EventRepository.h"
#import "RudderElementCache.h"
#import "Utils.h"
#import "RudderLogger.h"

static EventRepository* _instance;
BOOL initiated;

@implementation EventRepository

+ (instancetype)initiate:(NSString *)writeKey config:(RudderConfig *) config {
    if (_instance == nil) {
        _instance = [[self alloc] init:writeKey config:config];
    }
    
    return _instance;
}

/*
 * constructor to be called from RudderClient internally.
 * -- tasks to be performed
 * 1. persist the value of config
 * 2. initiate RudderElementCache
 * 3. initiate DBPersistentManager for SQLite operations
 * 4. initiate RudderServerConfigManager
 * 5. start processor thread
 * 6. initiate factories
 * */
- (instancetype)init : (NSString*) _writeKey config:(RudderConfig*) _config {
    self = [super init];
    if (self) {
        [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"EventRepository: writeKey: %@", _writeKey]];
        
        self->isFactoryInitialized = NO;
        
        writeKey = _writeKey;
        config = _config;
        
        NSData *authData = [[[NSString alloc] initWithFormat:@"%@:", _writeKey] dataUsingEncoding:NSUTF8StringEncoding];
        authToken = [authData base64EncodedStringWithOptions:0];
        [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"EventRepository: authToken: %@", authToken]];
        
        [RudderLogger logDebug:@"EventRepository: initiating element cache"];
        [RudderElementCache initiate];
        
        [RudderLogger logDebug:@"EventRepository: initiating dbPersistentManager"];
        dbpersistenceManager = [[DBPersistentManager alloc] init];
        
        [RudderLogger logDebug:@"EventRepository: initiating processor"];
        [self __initiateProcessor];
    }
    return self;
}

- (void)__initiateProcessor {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        [RudderLogger logDebug:@"processor started"];
        
        int sleepCount = 0;
        
        while (YES) {
            int recordCount = [self->dbpersistenceManager getDBRecordCount];
            [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"DBRecordCount %d", recordCount]];
            
            if (recordCount > self->config.dbCountThreshold) {
                [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"Old DBRecordCount %d", (recordCount - self->config.dbCountThreshold)]];
                RudderDBMessage *dbMessage = [self->dbpersistenceManager fetchEventsFromDB:(recordCount - self->config.dbCountThreshold)];
                [self->dbpersistenceManager clearEventsFromDB:dbMessage.messageIds];
            }
            
            [RudderLogger logDebug:@"Fetching events to flush to sever"];
            RudderDBMessage *dbMessage = [self->dbpersistenceManager fetchEventsFromDB:(self->config.flushQueueSize)];
            if (dbMessage.messages.count > 0 && (sleepCount >= self->config.sleepTimeout)) {
                NSString* payload = [self __getPayloadFromMessages:dbMessage.messages];
                [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"Payload: %@", payload]];
                if (payload != nil) {
                    NSString* response = [self __flushEventsToServer:payload];
                    [RudderLogger logInfo:[[NSString alloc] initWithFormat:@"Response: %@", response]];
                    [RudderLogger logInfo:[[NSString alloc] initWithFormat:@"EventCount: %lu", (unsigned long)dbMessage.messages.count]];
                    if (response != nil && [response  isEqual: @"OK"]) {
                        [RudderLogger logDebug:@"clearing events from DB"];
                        [self->dbpersistenceManager clearEventsFromDB:dbMessage.messageIds];
                        sleepCount = 0;
                    }
                }
            }
            [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"SleepCount: %d", sleepCount]];
            sleepCount += 1;
            usleep(1000000);
        }
    });
}

- (NSString*) __getPayloadFromMessages: (NSArray<NSString*>*) messages {
    NSString* sentAt = [Utils getTimestamp];
    [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"RecordCount: %lu", (unsigned long)messages.count]];
    [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"sentAtTimeStamp: %@", sentAt]];
    
    NSMutableString* json = [[NSMutableString alloc] init];
    
    [json appendString:@"{"];
    [json appendFormat:@"\"sentAt\":\"%@\",", sentAt];
    [json appendString:@"\"batch\":["];
    for (int index = 0; index < messages.count; index++) {
        NSMutableString* message = [[NSMutableString alloc] initWithString:messages[index]];
        long length = message.length;
        message = [[NSMutableString alloc] initWithString:[message substringWithRange:NSMakeRange(0, (length-1))]];
        [message appendFormat:@",\"sentAt\":\"%@\"}", sentAt];
        [json appendString:message];
        if (index != messages.count-1) {
            [json appendString:@","];
        }
    }
    [json appendString:@"]}"];
    
    return [json copy];
}

- (NSString*) __flushEventsToServer: (NSString*) payload {
    if (self->authToken == nil || [self->authToken isEqual:@""]) {
        [RudderLogger logError:@"WriteKey was not correct. Aborting flush to server"];
        return nil;
    }
    
    dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);
    
    __block NSString *responseStr = nil;
    NSString *endPointUrl = [self->config.endPointUrl stringByAppendingString:@"/v1/batch"];
    [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"endPointToFlush %@", endPointUrl]];
    
    NSMutableURLRequest *urlRequest = [[NSMutableURLRequest alloc] initWithURL:[[NSURL alloc] initWithString:endPointUrl]];
    [urlRequest setHTTPMethod:@"POST"];
    [urlRequest addValue:@"Application/json" forHTTPHeaderField:@"Content-Type"];
    [urlRequest addValue:[[NSString alloc] initWithFormat:@"Basic %@", self->authToken] forHTTPHeaderField:@"Authorization"];
    NSData *httpBody = [payload dataUsingEncoding:NSUTF8StringEncoding];
    [urlRequest setHTTPBody:httpBody];
    
    NSURLSession *session = [NSURLSession sharedSession];
    NSURLSessionDataTask *dataTask = [session dataTaskWithRequest:urlRequest completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
        NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *)response;
        
        [RudderLogger logError:[[NSString alloc] initWithFormat:@"statusCode %ld", (long)httpResponse.statusCode]];
        
        if (httpResponse.statusCode == 200) {
            if (data != nil) {
                responseStr = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            }
        } else {
            NSString *errorResponse = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            [RudderLogger logError:[[NSString alloc] initWithFormat:@"ServerError: %@", errorResponse]];
        }
        
        dispatch_semaphore_signal(semaphore);
    }];
    [dataTask resume];
    dispatch_semaphore_wait(semaphore, DISPATCH_TIME_FOREVER);
    
#if !__has_feature(objc_arc)
    dispatch_release(semaphore);
#endif
    
    return responseStr;
}

- (void) dump:(RudderMessage *)message {
    if (message == nil) return;
    
    message.integrations = @{@"All": @YES};
    
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:[message dict] options:0 error:nil];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    
    [RudderLogger logDebug:[[NSString alloc] initWithFormat:@"dump: %@", jsonString]];
    
    [self->dbpersistenceManager saveEvent:jsonString];
}

- (RudderConfig *)getConfig {
    return self->config;
}

@end
