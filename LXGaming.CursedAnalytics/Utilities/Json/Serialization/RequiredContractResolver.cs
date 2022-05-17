using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LXGaming.CursedAnalytics.Utilities.Json.Serialization; 

public class RequiredContractResolver : DefaultContractResolver {

    private readonly NullabilityInfoContext _nullabilityInfoContext;

    public RequiredContractResolver() {
        _nullabilityInfoContext = new NullabilityInfoContext();
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
        var property = base.CreateProperty(member, memberSerialization);

        NullabilityInfo? nullabilityInfo;
        lock (_nullabilityInfoContext) {
            nullabilityInfo = member switch {
                EventInfo eventInfo => _nullabilityInfoContext.Create(eventInfo),
                FieldInfo fieldInfo => _nullabilityInfoContext.Create(fieldInfo),
                PropertyInfo propertyInfo => _nullabilityInfoContext.Create(propertyInfo),
                _ => null
            };
        }

        property.Required = nullabilityInfo?.ReadState == NullabilityState.NotNull ? Required.Always : Required.Default;

        return property;
    }
}