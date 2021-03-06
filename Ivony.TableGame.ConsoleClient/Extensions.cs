﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.TableGame.ConsoleClient
{
  public static class Extensions
  {

    public static async Task<JObject> ReadAsJsonObjectAsync( this HttpContent content )
    {
      return JObject.Parse( await content.ReadAsStringAsync() );
    }

    public static async Task<JToken> ReadAsJsonAsync( this HttpContent content )
    {
      return JToken.Parse( await content.ReadAsStringAsync() );
    }

  }
}
