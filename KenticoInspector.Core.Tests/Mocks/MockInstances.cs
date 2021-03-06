﻿using System;

using KenticoInspector.Core.Instances.Models;

namespace KenticoInspector.Core.Tests.Mocks
{
    internal static class MockInstances
    {
        internal static Instance Kentico9 = new Instance
        {
            Name = "K9 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico9",
            Url = "http://kentico9.com",
            DatabaseSettings = null!
        };

        internal static Instance Kentico10 = new Instance
        {
            Name = "K10 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico10",
            Url = "http://kentico10.com",
            DatabaseSettings = null!
        };

        internal static Instance Kentico11 = new Instance
        {
            Name = "K11 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico11",
            Url = "http://kentico11.com",
            DatabaseSettings = null!
        };

        internal static Instance Kentico12 = new Instance
        {
            Name = "K11 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico12",
            Url = "http://kentico12.com",
            DatabaseSettings = null!
        };

        internal static Instance Get(int majorVersion)
        {
            return majorVersion switch
            {
                9 => Kentico9,
                10 => Kentico10,
                11 => Kentico11,
                12 => Kentico12,
                var _ => throw new Exception($"Version '{majorVersion}' not supported.")
            };
        }
    }
}