using Newtonsoft.Json;

namespace Wow.Blizzard.Client;

internal record ConnectedRealm(
    [property: JsonProperty("href")] string Href
);

internal record Key(
    [property: JsonProperty("href")] string Href
);

internal record Links(
    [property: JsonProperty("self")] Self Self
);

internal record Region(
    [property: JsonProperty("key")]  Key    Key,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("id")]   int    Id
);

internal record RealmResponse(
    [property: JsonProperty("_links")] Links  Links,
    [property: JsonProperty("id")]     int    Id,
    [property: JsonProperty("region")] Region Region,
    [property: JsonProperty("connected_realm")]
    ConnectedRealm ConnectedRealm,
    [property: JsonProperty("name")]     string    Name,
    [property: JsonProperty("category")] string    Category,
    [property: JsonProperty("locale")]   string    Locale,
    [property: JsonProperty("timezone")] string    Timezone,
    [property: JsonProperty("type")]     RealmType Type,
    [property: JsonProperty("is_tournament")]
    bool IsTournament,
    [property: JsonProperty("slug")] string Slug
);

internal record Self(
    [property: JsonProperty("href")] string Href
);

internal record RealmType(
    [property: JsonProperty("type")] string Type,
    [property: JsonProperty("name")] string Name
);