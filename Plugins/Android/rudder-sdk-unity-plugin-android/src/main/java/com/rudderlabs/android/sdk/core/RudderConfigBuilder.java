package com.rudderlabs.android.sdk.core;

import android.text.TextUtils;
import android.view.contentcapture.ContentCaptureSession;
import android.webkit.URLUtil;

import java.util.ArrayList;
import java.util.List;

/*
 * builder for RudderConfig
 *
 * isDebug -> true => set the logLevel to DEBUG automatically else sets as defined by the user
 * */
public class RudderConfigBuilder {
    private String endPointUri = Constants.BASE_URL;

    public RudderConfigBuilder withEndPointUri(String endPointUri) throws RudderException {
        if (TextUtils.isEmpty(endPointUri)) {
            throw new RudderException("endPointUri can not be null or empty");
        }
        if (!URLUtil.isValidUrl(endPointUri)) {
            throw new RudderException("malformed endPointUri");
        }
        this.endPointUri = endPointUri;
        return this;
    }

    private int flushQueueSize = Constants.FLUSH_QUEUE_SIZE;

    public RudderConfigBuilder withFlushQueueSize(int flushQueueSize) throws RudderException {
        if (flushQueueSize < 1 || flushQueueSize > 100) {
            throw new RudderException("flushQueueSize is out of range. Min: 1, Max: 100");
        }
        this.flushQueueSize = flushQueueSize;
        return this;
    }

    private boolean isDebug = false;

    public RudderConfigBuilder withDebug(boolean isDebug) {
        this.isDebug = isDebug;
        return this;
    }

    private int logLevel = RudderLogger.RudderLogLevel.NONE;

    public RudderConfigBuilder withLogLevel(int logLevel) {
        this.logLevel = logLevel;
        return this;
    }

    private int dbThresholdCount = Constants.DB_COUNT_THRESHOLD;

    public RudderConfigBuilder withDbThresholdCount(int dbThresholdCount) {
        this.dbThresholdCount = dbThresholdCount;
        return this;
    }

    private int sleepTimeout = Constants.SLEEP_TIMEOUT;

    public RudderConfigBuilder withSleepCount(int sleepCount) {
        this.sleepTimeout = sleepCount;
        return this;
    }

    public RudderConfig build() throws RudderException {
        return new RudderConfig(endPointUri, flushQueueSize, dbThresholdCount, sleepTimeout, isDebug ? RudderLogger.RudderLogLevel.DEBUG : logLevel);
    }
}
