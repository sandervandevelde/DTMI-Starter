using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.IoT.ModelsRepository;
using Microsoft.Azure.DigitalTwins.Parser;
using Azure.Core.Diagnostics;

namespace dtmitest
{
    /// <summary>
    /// Nuget: https://www.nuget.org/packages/Azure.IoT.ModelsRepository
    /// Nuget code samples: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/modelsrepository/Azure.IoT.ModelsRepository/samples
    /// </summary>
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello DTMI World!");

            // note: 'master' is the publicly available branch
            var uri = new Uri("https://raw.githubusercontent.com/sandervandevelde/DTMI-Starter/master");

            var dtmi = "dtmi:com:example:TemperatureController;1";

            var isValidDtmi = DtmiConventions.IsValidDtmi(dtmi);

            Console.WriteLine($"isValidDtmi: {isValidDtmi}");

            var modelUri = DtmiConventions.GetModelUri(dtmi, uri, false);

            var clientRaw = new ModelsRepositoryClient(
                    uri,
                    new ModelsRepositoryClientOptions(dependencyResolution: ModelDependencyResolution.Enabled));

            IDictionary<string, string> models = clientRaw.GetModels(dtmi);

            var parser = new ModelParser();
            IReadOnlyDictionary<Dtmi, DTEntityInfo> parseResult = await parser.ParseAsync(models.Values.ToArray());

            Console.WriteLine($"The following {models.Count} interfaces are resolved with {parseResult.Count} entities.");

            models.Keys.ToList().ForEach(k => Console.WriteLine($"Model: {k}"));

            parseResult.Values.ToList().ForEach(v => Console.WriteLine($"element: {v.GetType()}"));

            //// *** Alternative way to parse. Notice the additional extension method in this project ***
            //var parser = new ModelParser
            //{
            //    // Usage of the ModelsRepositoryClientExtensions.ParserDtmiResolver extension.
            //    DtmiResolver = client.ParserDtmiResolver
            //};
            //IReadOnlyDictionary<Dtmi, DTEntityInfo> parseResult = await parser.ParseAsync(models.Values.Take(1).ToArray());
            //Console.WriteLine($"{dtmi} resolved in {models.Count} interfaces with {parseResult.Count} entities."); // 38 entities

            foreach (var p in parseResult)
            {
                if (p.Value is DTTelemetryInfo)
                {
                    Console.WriteLine($"parse '{p.Value}': '{p.Key}' ");
                    Console.WriteLine($"  Id '{p.Value.Id.AbsolutePath}' ");
                    Console.WriteLine($"  DisplayName '{p.Value.DisplayName.First().Key}' - '{p.Value.DisplayName.First().Value}'");
                    Console.WriteLine($"  EntityKind {p.Value.EntityKind}");
                    Console.WriteLine($"  Description {p.Value.Description.First()}");
                    Console.WriteLine($"  DefinedIn {p.Value.DefinedIn.AbsolutePath}");
                    Console.WriteLine($"  Comment {p.Value.Comment?.First()}");

                    var pt = p.Value as DTTelemetryInfo;
                    Console.WriteLine($"  pt.EntityKind = {pt.EntityKind}");

                    if (pt.Schema is DTDoubleInfo)
                    {
                        var ptdouble = pt.Schema as DTDoubleInfo;

                        Console.WriteLine($"  ptdouble {ptdouble.EntityKind} - {ptdouble.Description.First().Value} ");
                    }
                }

                // if (p.Value is DTCommandInfo)
                // {
                //     Console.WriteLine($"parse key {p.Key} {p.Value} Id {p.Value.Id.AbsolutePath} DisplayName {p.Value.DisplayName.First().Key} {p.Value.DisplayName.First().Value}");
                //     Console.WriteLine($"  EntityKind {p.Value.EntityKind.ToString()}");
                //     Console.WriteLine($"  Description {p.Value.Description.First()}");
                //     Console.WriteLine($"  DefinedIn {p.Value.DefinedIn.AbsolutePath}");
                //     Console.WriteLine($"  Comment {p.Value.Comment?.First()}");
                // }

                // if (p.Value is DTPropertyInfo)
                // {
                //     Console.WriteLine($"parse key {p.Key} {p.Value} Id {p.Value.Id.AbsolutePath} DisplayName {p.Value.DisplayName.First().Key} {p.Value.DisplayName.First().Value}");
                //     Console.WriteLine($"  EntityKind {p.Value.EntityKind.ToString()}");
                //     Console.WriteLine($"  Description {p.Value.Description.First()}");
                //     Console.WriteLine($"  DefinedIn {p.Value.DefinedIn.AbsolutePath}");
                //     Console.WriteLine($"  Comment {p.Value.Comment?.First()}");
                // }
            }

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();
        }
    }
}