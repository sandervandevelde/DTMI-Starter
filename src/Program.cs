using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.IoT.ModelsRepository;
using Microsoft.Azure.DigitalTwins.Parser;

namespace dtmitest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var client = new ModelsRepositoryClient();
            Console.WriteLine($"Initialized client pointing to global endpoint: {client.RepositoryUri}");

            // The output of GetModelsAsync() will include at least the definition for the target dtmi.
            // If the model dependency resolution configuration is not disabled, then models in which the
            // target dtmi depends on will also be included in the returned IDictionary<string, string>.
            var dtmi = "dtmi:com:example:TemperatureController;1";
            IDictionary<string, string> models = await client.GetModelsAsync(dtmi).ConfigureAwait(false);

            // In this case the above dtmi has 2 model dependencies.
            // dtmi:com:example:Thermostat;1 and dtmi:azure:DeviceManagement:DeviceInformation;1
            Console.WriteLine($"{dtmi} resolved in {models.Count} interfaces.");

            foreach(var m in models)
            {
                System.Console.WriteLine($"Model {m.Key}");
            }


            // var parser = new ModelParser();
            // IReadOnlyDictionary<Dtmi, DTEntityInfo> parseResult = await parser.ParseAsync(models.Values.ToArray());
            // Console.WriteLine($"{dtmi} resolved in {models.Count} interfaces with {parseResult.Count} entities.");                        
            
            var parser = new ModelParser
            {
                // Usage of the ModelsRepositoryClientExtensions.ParserDtmiResolver extension.
                DtmiResolver = client.ParserDtmiResolver
            };

            IReadOnlyDictionary<Dtmi, DTEntityInfo> parseResult = await parser.ParseAsync(models.Values.Take(1).ToArray());
            Console.WriteLine($"{dtmi} resolved in {models.Count} interfaces with {parseResult.Count} entities."); // 38 entities

            foreach(var p in parseResult)
            {
                if (p.Value is Microsoft.Azure.DigitalTwins.Parser.DTTelemetryInfo)
                {
                    System.Console.WriteLine($"parse key '{p.Key}' ");// : '{p.Value}' Id {p.Value.Id.AbsolutePath} DisplayName {p.Value.DisplayName.First().Key} {p.Value.DisplayName.First().Value}");
                    // System.Console.WriteLine($"  EntityKind {p.Value.EntityKind.ToString()}");
                    // //System.Console.WriteLine($"  Description {p.Value.Description.First()}");
                    System.Console.WriteLine($"  DefinedIn {p.Value.DefinedIn.AbsolutePath}");
                    // ////System.Console.WriteLine($"Comment {p.Value.Comment.First()}");

                    var pt = p.Value as Microsoft.Azure.DigitalTwins.Parser.DTTelemetryInfo;

                    System.Console.WriteLine($"pt.EntityKind = {pt.EntityKind}");

                    if (pt.Schema is Microsoft.Azure.DigitalTwins.Parser.DTDoubleInfo) 
                    {
                        var ptdouble = pt.Schema as Microsoft.Azure.DigitalTwins.Parser.DTDoubleInfo;

                        System.Console.WriteLine(  $"ptdouble {ptdouble.EntityKind} - {ptdouble.Description.First().Value} " );
                    }

                }

                // if (p.Value is Microsoft.Azure.DigitalTwins.Parser.DTCommandInfo)
                // {
                //     System.Console.WriteLine($"parse key {p.Key} {p.Value} Id {p.Value.Id.AbsolutePath} DisplayName {p.Value.DisplayName.First().Key} {p.Value.DisplayName.First().Value}");
                //     System.Console.WriteLine($"  EntityKind {p.Value.EntityKind.ToString()}");
                //     //System.Console.WriteLine($"  Description {p.Value.Description.First()}");
                //     System.Console.WriteLine($"  DefinedIn {p.Value.DefinedIn.AbsolutePath}");
                //     ////System.Console.WriteLine($"Comment {p.Value.Comment.First()}"); 
                // }

                // if (p.Value is Microsoft.Azure.DigitalTwins.Parser.DTPropertyInfo)
                // {
                //     System.Console.WriteLine($"parse key {p.Key} {p.Value} Id {p.Value.Id.AbsolutePath} DisplayName {p.Value.DisplayName.First().Key} {p.Value.DisplayName.First().Value}");
                //     System.Console.WriteLine($"  EntityKind {p.Value.EntityKind.ToString()}");
                //     //System.Console.WriteLine($"  Description {p.Value.Description.First()}");
                //     System.Console.WriteLine($"  DefinedIn {p.Value.DefinedIn.AbsolutePath}");
                //     ////System.Console.WriteLine($"Comment {p.Value.Comment.First()}");
                // }
            }

            //System.Console.WriteLine("Press a key to exit");
            //System.Console.ReadKey();
        }
    }
}
