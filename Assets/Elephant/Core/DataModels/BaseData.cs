using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElephantSDK
{
    [Serializable]
    public class BaseData
    {
#if UNITY_IOS
        public string platform = "ios";
#elif UNITY_ANDROID
        public string platform = "android";
#else
        public string platform = "editor";
#endif

        public string idfa;
        public string idfv;
        public string bundle;
        public string lang;
        public string app_version;
        public string build_number;
        public string device_cpu_arch;
        public string os_version;
        public string sdk_version;
        public string ad_sdk_version;
        public string device_model;
        public string user_tag;
        public long create_date;
        public long session_id;
        public long real_session_id;
        public float real_time_since_start_up;
        public long first_install_time;
        public string timezone_offset = "";
        public string user_id;
        public string client_id;
        public string consent_status = "";
        public int order = 0;
        public List<MirrorData> mirror_data;
        public string adjustId = "";
        [FormerlySerializedAs("networName")] public string networkName = "";
        public string campaignName = "";
        public string adGroupName = "";
        public string creativeName = "";
        public double uaCost;

        public void FillBaseData(long sessionID)
        {
                this.bundle = Application.identifier;
                this.idfa = ElephantCore.Instance.idfa;
                this.idfv = ElephantCore.Instance.idfv;
                this.app_version = Application.version;
                this.build_number = ElephantCore.Instance.buildNumber;
                this.device_cpu_arch = Utils.GetDeviceCpuArch();
                this.lang = Utils.GetISOCODE(Application.systemLanguage);
                this.user_tag = RemoteConfig.GetInstance().GetTag();
                this.os_version = SystemInfo.operatingSystem;
                this.sdk_version = ElephantVersion.SDK_VERSION;
                this.ad_sdk_version = VersionCheckUtils.GetInstance().AdSdkVersion;
                this.device_model = SystemInfo.deviceModel;
                this.create_date = Utils.Timestamp();
                this.session_id = sessionID;
                this.real_session_id = ElephantCore.Instance.realSessionId;
                this.real_time_since_start_up = Time.realtimeSinceStartup;
                this.first_install_time = ElephantCore.Instance.firstInstallTime;
                this.user_id = ElephantCore.Instance.userId;
                this.client_id = ElephantCore.Instance.clientId;
                this.consent_status = ElephantCore.Instance.consentStatus;
                this.mirror_data = ElephantCore.Instance.mirrorData;
                this.adjustId = ElephantCore.Instance.adjustId;
                this.networkName = ElephantCore.Instance.networkName;
                this.campaignName = ElephantCore.Instance.campaignName;
                this.adGroupName = ElephantCore.Instance.adGroupName;
                this.creativeName = ElephantCore.Instance.creativeName;
                this.uaCost = ElephantCore.Instance.uaCost;

                this.order = ElephantCore.Instance.eventOrder;
                ElephantCore.Instance.eventOrder++;

                try
                {
                        TimeZone localZone = TimeZone.CurrentTimeZone;
                        DateTime currentDate = DateTime.Now;
                        TimeSpan currentOffset = 
                                localZone.GetUtcOffset( currentDate );
                        this.timezone_offset = currentOffset.ToString();
                }
                catch (Exception e)
                {
                        Debug.Log(e);
                }
                
        }

        public void RefreshBaseData()
        {
                this.idfa = ElephantCore.Instance.idfa;
                this.idfv = ElephantCore.Instance.idfv;
                this.lang = Utils.GetISOCODE(Application.systemLanguage);
                this.user_tag = RemoteConfig.GetInstance().GetTag();
                this.real_time_since_start_up = Time.realtimeSinceStartup;
                this.consent_status = ElephantCore.Instance.consentStatus;
        }
    }
}