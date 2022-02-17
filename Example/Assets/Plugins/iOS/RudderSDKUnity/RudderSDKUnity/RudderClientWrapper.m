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
        [RSClient putAnonymousId: _anonymousId];
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
  
// if(_userPropsJson != nil) {
//     NSDictionary<NSString*, NSObject*>* userTraitsDict = [self _convertToDict:_userPropsJson];
//     RSTraits *traits = [[RSTraits alloc] initWithDict:userTraitsDict];
//     [builder setTraits:traits];
// }

if(_optionsJson != nil) {
     NSDictionary<NSString*, NSObject*>* optionsDict = [self _convertToDict:_optionsJson];
     RSOption *option = [self _getRudderOptionsObject:optionsDict];
     [builder setRSOption:option];
}

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
    
    RSOption *option = nil;
    if(_optionsJson != nil) {
      NSDictionary<NSString*, NSObject*>* optionsDict = [self _convertToDict:_optionsJson];
      option = [self _getRudderOptionsObject:optionsDict];
    }
    // make the identify call
    [_rudderClient identify:_userId traits:traitsDict options:option];
}

+ (void)_reset {
    if (_rudderClient == nil) return;
    [_rudderClient reset];
}

+ (void) _setAnonymousId:(NSString *)_anonymousId {
    [RSLogger logDebug:[[NSString alloc] initWithFormat:@"_setAnonymousId: %@", _anonymousId]];
    if(_anonymousId != nil) {
    [RSClient putAnonymousId:_anonymousId];
    }
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

+ (RSOption*) _getRudderOptionsObject:(NSDictionary *) optionsDict {
    RSOption * options = [[RSOption alloc]init];
    if([optionsDict objectForKey:@"externalIds"])
    {
        NSArray *externalIdsArray =  [optionsDict objectForKey:@"externalIds"];
        for(NSDictionary *externalId in externalIdsArray) {
            [options putExternalId:[externalId objectForKey:@"type"] withId:[externalId objectForKey:@"id"]];
        }
    }
    if([optionsDict objectForKey:@"integrations"])
    {
        NSDictionary *integrationsDict = [optionsDict objectForKey:@"integrations"];
        for(NSString* key in integrationsDict)
        {
            [options putIntegration:key isEnabled:[[integrationsDict objectForKey:key] boolValue]];
        }
    }
    return options;
}

@end
