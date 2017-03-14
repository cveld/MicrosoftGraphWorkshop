using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoApiWeb.Models.JsonHelpers
{
    public class Group
    {
    }

    public class GroupCollection
    {
        public string odatacontext { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public object classification { get; set; }
        public object createdDateTime { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public string[] groupTypes { get; set; }
        public string mail { get; set; }
        public bool mailEnabled { get; set; }
        public string mailNickname { get; set; }
        public object onPremisesLastSyncDateTime { get; set; }
        public object onPremisesSecurityIdentifier { get; set; }
        public object onPremisesSyncEnabled { get; set; }
        public string[] proxyAddresses { get; set; }
        public object renewedDateTime { get; set; }
        public bool securityEnabled { get; set; }
        public string visibility { get; set; }
    }
}