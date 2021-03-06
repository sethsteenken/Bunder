﻿using Moq;
using System.Collections.Generic;

namespace Bunder.Tests
{
    internal static class AssetResolverTestHelper
    {
        public static AssetResolver BuildTestResolver(
            IBundleLookup bundleLookup = null, 
            IPathFormatter pathFormatter = null,
            IBunderCache cache = null,
            BunderSettings settings = null)
        {
            if (bundleLookup == null)
                bundleLookup = new Mock<IBundleLookup>().Object;

            if (pathFormatter == null)
                pathFormatter = new MockPathFormatter();

            if (settings == null)
                settings = new BunderSettings();

            if (cache == null)
                cache = new Mock<IBunderCache>().Object;

            return new AssetResolver(bundleLookup, pathFormatter, cache, settings);
        }

        public static AssetResolutionContext BuildValidResolutionContext()
        {
            return new AssetResolutionContext(new List<string>()
            {
                "/my/source/one.js",
                "my-bundle"
            },
            useBundledOutput: true,
            includeVersioning: true);
        }
    }
}
