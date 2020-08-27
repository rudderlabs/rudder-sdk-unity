package com.rudderstack.android.sdk.core;

import com.google.gson.annotations.SerializedName;
import com.rudderstack.android.sdk.BuildConfig;

class RudderLibraryInfo {
    @SerializedName("name")
    private String name = BuildConfig.LIBRARY_PACKAGE_NAME;
    @SerializedName("version")
    private String version = "1.0.2-patch.3";
}
