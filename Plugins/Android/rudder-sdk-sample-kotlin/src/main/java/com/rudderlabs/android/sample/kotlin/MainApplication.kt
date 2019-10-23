package com.rudderlabs.android.sample.kotlin

import android.app.Application
import com.rudderlabs.android.sdk.core.RudderClient

class MainApplication : Application() {
    companion object {
        private const val WRITE_KEY = "1R3JbxsqWZlbYjJlBxf0ZNWZOH6"
        private const val END_POINT_URI = "https://2f0d770f.ngrok.io"
    }

    override fun onCreate() {
        super.onCreate()

        RudderClient._initiateInstance(
            this,
            WRITE_KEY,
            END_POINT_URI,
            30,
            10000,
            10
        )
    }
}