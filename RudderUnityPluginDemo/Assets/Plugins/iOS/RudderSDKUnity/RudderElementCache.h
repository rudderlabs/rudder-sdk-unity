//
//  RudderElementCache.h
//  RudderSDKCore
//
//  Created by Arnab Pal on 17/10/19.
//  Copyright © 2019 RudderStack. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RudderContext.h"

NS_ASSUME_NONNULL_BEGIN

@interface RudderElementCache : NSObject

+ (void) initiate;

+ (RudderContext*) getContext;

+ (void) updateTraits : (RudderTraits*) traits;

+ (void) persistTraits;

+ (void) reset;

+ (void) updateTraitsDict: (NSMutableDictionary<NSString*, NSObject*> *) traitsDict;

+ (void) setAnonymousId: (NSString*) anonymousId;

+ (NSString*) getAnonymousId;

@end

NS_ASSUME_NONNULL_END
