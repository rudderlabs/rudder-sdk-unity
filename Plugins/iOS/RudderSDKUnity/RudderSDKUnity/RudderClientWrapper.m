//
//  RudderClientWrapper.m
//  Pods-RudderSDKUnityTest
//
//  Created by Arnab Pal on 15/12/19.
//

#import "RudderClientWrapper.h"
#import "RSClient.h"
#import "RSUtils.h"
#import "RSLogger.h"
#import "RSElementCache.h"

static RSClient *_rudderClient;

@implementation RudderClientWrapper

+ (void)_initiateInstance:(NSString *)_anonymousId
                 writeKey:(NSString *)_writeKey
             dataPlaneUrl:(NSString *)_dataPlaneUrl
          controlPlaneUrl:(NSString *)_controlPlaneUrl
           flushQueueSize:(int)_flushQueueSize
         dbCountThreshold:(int)_dbCountThreshold
             sleepTimeOut:(int)_sleepTimeout
    configRefreshInterval:(int)_configRefreshInterval
     trackLifecycleEvents:(BOOL)_trackLifecycleEvents
        recordScreenViews:(BOOL)_recordScreenViews
                 logLevel:(int)_logLevel {
    if (_rudderClient == nil) {
        [RSElementCache setAnonymousId:_anonymousId];
        RSConfigBuilder *builder = [[RSConfigBuilder alloc] init];
        [builder withDataPlaneUrl:_dataPlaneUrl];
        [builder withControlPlaneUrl:_controlPlaneUrl];
        [builder withFlushQueueSize:_flushQueueSize];
        [builder withDBCountThreshold:_dbCountThreshold];
        [builder withSleepTimeOut:_sleepTimeout];
        [builder withConfigRefreshInteval:_configRefreshInterval];
        [builder withTrackLifecycleEvens:_trackLifecycleEvents];
        [builder withRecordScreenViews:_recordScreenViews];
        [builder withLoglevel:_logLevel];
        _rudderClient = [RSClient getInstance:_writeKey config:[builder build]];
    }
}

+ (void)_logEvent:(NSString *)_eventType
        eventName:(NSString *)_eventName
   eventPropsJson:(NSString *)_eventPropsJson
    userPropsJson:(NSString *)_userPropsJson
      optionsJson:(NSString *)_optionsJson
{
    if (_rudderClient == nil) return;
    
    RSMessageBuilder *builder = [[RSMessageBuilder alloc] init];
    [builder setEventName:_eventName];
    [builder setPropertyDict:[self _convertToDict:_eventPropsJson]];
    [builder setUserProperty:[self _convertToDict:_userPropsJson]];
//    [builder setRudderOption:[self _convertToDict:_optionsJson]];
    
    if ([_eventType isEqualToString:@"track"]) {
        [_rudderClient trackMessage:[builder build]];
    } else if ([_eventType isEqualToString:@"screen"]) {
        [_rudderClient screenWithMessage:[builder build]];
    } else {
        [RSLogger logError:@"message type is not supported"];
    }
    
}

+ (void)_identify:(NSString *)_userId traitsJson:(NSString *)_traitsJson optionsJson:(NSString *)_optionsJson {
    NSDictionary *traitsDict = [self _convertToDict:_traitsJson];
    if (traitsDict == nil) {
        // if traits is not filled in, fill with anonymousId
        traitsDict = @{@"anonymousId": [RSElementCache getAnonymousId]};
    } else {
        // if anonymousId is not filled in
        NSString *anonymoysId = [traitsDict valueForKey:@"anonymousId"];
        if (anonymoysId == nil) {
            [[traitsDict mutableCopy] setObject:[RSElementCache getAnonymousId] forKey:@"anonymousId"];
        }
    }
    NSDictionary *optinsDict = [self _convertToDict:_optionsJson];
    // make the identify call
    [_rudderClient identify:_userId traits:traitsDict options:optinsDict];
}

+ (void)setAnonymousId:(NSString *)_anonymousId {
    if(anonymousId != nil) {
    [RSClient putAnonymousId:anonymousId];
    }
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
