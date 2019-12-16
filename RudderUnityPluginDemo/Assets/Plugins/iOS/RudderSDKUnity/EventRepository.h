//
//  EventRepository.h
//  RudderSDKCore
//
//  Created by Arnab Pal on 17/10/19.
//  Copyright © 2019 Rudderlabs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RudderMessage.h"
#import "DBPersistentManager.h"
#import "RudderConfig.h"

NS_ASSUME_NONNULL_BEGIN

@interface EventRepository : NSObject {
    NSString* writeKey;
    NSString* authToken;
    RudderConfig* config;
    DBPersistentManager* dbpersistenceManager;
    BOOL isFactoryInitialized;
}

+ (instancetype) initiate: (NSString*) writeKey config: (RudderConfig*) config;
- (void) dump:(RudderMessage*) message;
- (void) __initiateProcessor;
- (NSString*) __getPayloadFromMessages: (NSArray*) messages;
- (NSString*) __flushEventsToServer: (NSString*) payload;

- (RudderConfig* _Nullable) getConfig;

@end

NS_ASSUME_NONNULL_END
