﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.IoT.ModelsRepository;
using Microsoft.Azure.DigitalTwins.Parser;
using Azure.Core.Diagnostics;

namespace dtmitest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello DTMI World!");

            //using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();

            var clientRaw = new ModelsRepositoryClient(
                new Uri("https://raw.githubusercontent.com/sandervandevelde/DTMI-Starter/master"),
                //new Uri("https://raw.githubusercontent.com/Azure/iot-plugandplay-models/main"),
                new ModelsRepositoryClientOptions(dependencyResolution: ModelDependencyResolution.Enabled));

            IDictionary<string, string> models = clientRaw.GetModels("dtmi:com:example:TemperatureController;1");

            models.Keys.ToList().ForEach(k => Console.WriteLine(k));

            //var client = new ModelsRepositoryClient();
            //Console.WriteLine($"Initialized client pointing to global endpoint: {client.RepositoryUri}");

            var dtmi = "dtmi:com:example:TemperatureController;1";
            //IDictionary<string, string> models = await client.GetModelsAsync(dtmi).ConfigureAwait(false);

            //// In this case the above dtmi has 2 model dependencies.
            //// dtmi:com:example:Thermostat;1 and dtmi:azure:DeviceManagement:DeviceInformation;1
            //Console.WriteLine($"{dtmi} resolved in {models.Count} interfaces.");

            //foreach (var m in models)
            //{
            //    Console.WriteLine($"Model {m.Key}");
            //}

            var parser = new ModelParser();
            IReadOnlyDictionary<Dtmi, DTEntityInfo> parseResult = await parser.ParseAsync(models.Values.ToArray());
            Console.WriteLine($"{dtmi} resolved in {models.Count} interfaces with {parseResult.Count} entities.");

            foreach (var p in parseResult)
            {
                Console.WriteLine($"Model {p.Value.GetType()}");
            }

            //var parser = new ModelParser
            //{
            //    // Usage of the ModelsRepositoryClientExtensions.ParserDtmiResolver extension.
            //    DtmiResolver = client.ParserDtmiResolver
            //};

            //IReadOnlyDictionary<Dtmi, DTEntityInfo> parseResult = await parser.ParseAsync(models.Values.Take(1).ToArray());
            //Console.WriteLine($"{dtmi} resolved in {models.Count} interfaces with {parseResult.Count} entities."); // 38 entities

            foreach (var p in parseResult)
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

                        System.Console.WriteLine($"ptdouble {ptdouble.EntityKind} - {ptdouble.Description.First().Value} ");
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