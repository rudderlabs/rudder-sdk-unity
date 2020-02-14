//
//  UnityBridge.h
//  RudderSDKUnity
//
//  Created by Arnab Pal on 23/10/19.
//  Copyright © 2019 RudderStack. All rights reserved.
//

#import "RudderSDKUnity.h"

#ifdef __cplusplus
extern "C"
{
#endif
    void _initiateInstance(
        const char *_anonymousId,
        const char *_writeKey,
        const char *_endPointUrl,
        const int _flushQueueSize,
        const int _dbCountThreshold,
        const int _sleepTimeout,
        const int _logLevel);

    void _logEvent(const char *_eventType,
                   const char *_eventName,
                   const char *_eventPropsJson,
                   const char *_userPropsJson,
                   const char *_optionsJson);

    void _identify(const char *_userId,
                   const char *_traitsJson,
                   const char *_optionsJson);

    void _reset();

#ifdef __cplusplus
}
#endif
