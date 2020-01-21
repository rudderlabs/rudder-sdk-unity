//
//  RudderClientWrapper.m
//  Pods-RudderSDKUnityTest
//
//  Created by Arnab Pal on 15/12/19.
//

#import "RudderClientWrapper.h"
#import "RudderClient.h"
#import "Utils.h"
#import "RudderLogger.h"
#import "RudderElementCache.h"

static RudderClient *_rudderClient;

@implementation RudderClientWrapper

+ (void)_initiateInstance:(NSString *)_anonymousId
                 writeKey:(NSString *)_writeKey
              endPointUrl:(NSString *)_endPointUrl
           flushQueueSize:(int)_flushQueueSize
         dbCountThreshold:(int)_dbCountThreshold
             sleepTimeOut:(int)_sleepTimeout
                 logLevel:(int)_logLevel {
    if (_rudderClient == nil) {
        [RudderElementCache setAnonymousId:_anonymousId];
        RudderConfigBuilder *builder = [[RudderConfigBuilder alloc] init];
        [builder withEndPointUrl:_endPointUrl];
        [builder withFlushQueueSize:_flushQueueSize];
        [builder withDBCountThreshold:_dbCountThreshold];
        [builder withSleepTimeOut:_sleepTimeout];
        [builder withLoglevel:_logLevel];
        _rudderClient = [RudderClient getInstance:_writeKey config:[builder build]];
    }
}

+ (void)_logEvent:(NSString *)_eventType
        eventName:(NSString *)_eventName
   eventPropsJson:(NSString *)_eventPropsJson
    userPropsJson:(NSString *)_userPropsJson
      optionsJson:(NSString *)_optionsJson
{
    if (_rudderClient == nil) return;
    
    RudderMessageBuilder *builder = [[RudderMessageBuilder alloc] init];
    [builder setEventName:_eventName];
    [builder setPropertyDict:[self _convertToDict:_eventPropsJson]];
    [builder setUserProperty:[self _convertToDict:_userPropsJson]];
//    [builder setRudderOption:[self _convertToDict:_optionsJson]];
    
    if ([_eventType isEqualToString:@"track"]) {
        [_rudderClient trackMessage:[builder build]];
    } else if ([_eventType isEqualToString:@"screen"]) {
        [_rudderClient screenWithMessage:[builder build]];
    } else {
        [RudderLogger logError:@"message type is not supported"];
    }
    
}

+ (void)_identify:(NSString *)_userId traitsJson:(NSString *)_traitsJson optionsJson:(NSString *)_optionsJson {
    NSDictionary *traitsDict = [self _convertToDict:_traitsJson];
    if (traitsDict == nil) {
        // if traits is not filled in, fill with anonymousId
        traitsDict = @{@"anonymousId": [[[[UIDevice currentDevice] identifierForVendor] UUIDString]lowercaseString]};
    } else {
        // if anonymousId is not filled in
        NSString *anonymoysId = [traitsDict valueForKey:@"anonymousId"];
        if (anonymoysId == nil) {
            [[traitsDict mutableCopy] setObject:[[[[UIDevice currentDevice] identifierForVendor] UUIDString]lowercaseString] forKey:@"anonymousId"];
        }
    }
    NSDictionary *optinsDict = [self _convertToDict:_optionsJson];
    // make the identify call
    [_rudderClient identify:_userId traits:traitsDict options:optinsDict];
}

+ (void)_reset {
    if (_rudderClient == nil) return;
    [_rudderClient reset];
}

+ (NSDictionary<NSString*, NSObject*>*) _convertToDict: (NSString*) _json {
    NSStringEncoding encoding = 0;
    NSData *jsonData = [_json dataUsingEncoding:encoding];
    NSError *error = nil;
    NSDictionary *parsedData = [NSJSONSerialization JSONObjectWithData:jsonData options:kNilOptions error:&error];
    if (error == nil) {
        return parsedData;
    } else {
        return nil;
    }
}

@end
