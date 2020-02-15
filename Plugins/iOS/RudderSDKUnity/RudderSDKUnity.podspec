Pod::Spec.new do |spec|
  spec.name         = "RudderSDKUnity"
  spec.version      = "1.0"
  spec.summary      = "A short description of RudderSDKCore."
  spec.description  = <<-DESC
Rudder is a platform for collecting, storing and routing customer event data to dozens of tools. Rudder is open-source, can run in your cloud environment (AWS, GCP, Azure or even your data-centre) and provides a powerful transformation framework to process your event data on the fly.
                   DESC

  spec.homepage     = "https://github.com/rudderlabs/rudder-sdk-unity"
  spec.license      = { :type => "MIT", :file => "FILE_LICENSE" }
  spec.author       = { "RudderStack" => "arnab@rudderlabs.com" }
  spec.platform     = :ios
  spec.platform     = :ios, "9.0"
  spec.source       = { :git => "https://github.com/rudderlabs/rudder-sdk-unity.git" }

  spec.source_files  = "Classes", "**/*.{h,m}"
  spec.exclude_files = "Classes/Exclude"
end
