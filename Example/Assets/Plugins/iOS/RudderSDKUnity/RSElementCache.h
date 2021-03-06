//
//  RSElementCache.h
//  RSSDKCore
//
//  Created by Arnab Pal on 17/10/19.
//  Copyright © 2019 RudderStack. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RSContext.h"

NS_ASSUME_NONNULL_BEGIN

@interface RSElementCache : NSObject

+ (void) initiate;

+ (RSContext*) getContext;

+ (NSString*) getAnonymousId;

+ (void) updateTraits : (RSTraits*) traits;

+ (void) persistTraits;

+ (void) reset;

+ (void) updateTraitsDict: (NSMutableDictionary<NSString*, NSObject*> *) traitsDict;

+ (void) setAnonymousId: (NSString *) anonymousId;

@end

NS_ASSUME_NONNULL_END
