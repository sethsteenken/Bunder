﻿using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bunder.Tests
{
    public class AssetResolverTests
    {
        [Fact]
        public void Resolve_ContextParam_ThrowsException_WhenContextIsNull()
        {
            var assetResolver = AssetResolverTestHelper.BuildTestResolver();
            Assert.Throws<ArgumentNullException>(() => assetResolver.Resolve(context: null));
        }

        [Fact]
        public void Resolve_ContextParam_DoesNotReturnNull_WhenValidContextWithNullPathsIsSupplied()
        {
            var assetResolver = AssetResolverTestHelper.BuildTestResolver();
            var context = new AssetResolutionContext(null, useBundledOutput: true, includeVersioning: true);

            var assets = assetResolver.Resolve(context);

            Assert.NotNull(assets);
        }

        [Fact]
        public void Resolve_ContextParam_DoesNotReturnNull_WhenValidContextWithPathsIsSupplied()
        {
            var assetResolver = AssetResolverTestHelper.BuildTestResolver();
            var context = AssetResolverTestHelper.BuildValidResolutionContext();

            var assets = assetResolver.Resolve(context);

            Assert.NotNull(assets);
        }

        [Theory]
        [InlineData("my-bundle", "/my/source.js")]
        [InlineData("my-bundle", "/my/source.js", "some-other-bundle", "another-bundle")]
        public void Resolve_ContextParam_ReturnsCorrectCount_WhenBundledOutputTrue(params string[] pathsOrBundles)
        {                                    
            var assetResolver = AssetResolverTestHelper.BuildTestResolver();

            var context = new AssetResolutionContext(pathsOrBundles, useBundledOutput: true, includeVersioning: true);

            var assets = assetResolver.Resolve(context);

            Assert.Equal(pathsOrBundles?.Count() ?? 0, assets.Count());
        }

        [Theory]
        [InlineData("my-bundle", "one.js", "two.js")]
        [InlineData("my-other-bundle", "one-source-file.js")]
        public void Resolve_ContextParam_ReturnsBundledFiles_WhenValidBundleIsFoundAndUseBundledOutputFalse(string bundleName, params string[] files)
        {
            var bundle = new Bundle(bundleName, "js", "/my/output-directory", files);
            var bundleLookup = new Mock<IBundleLookup>();
            bundleLookup.Setup(bl => bl.TryGetBundle(bundleName, out bundle)).Returns(true);

            var assetResolver = AssetResolverTestHelper.BuildTestResolver(bundleLookup.Object);

            var context = new AssetResolutionContext(new[] { bundleName }, useBundledOutput: false, includeVersioning: true);

            var assets = assetResolver.Resolve(context);

            Assert.Equal(bundle.Files, assets.Select(a => a.Value));
        }

        [Theory]
        [InlineData("my-bundle", "one.js", "two.js")]
        [InlineData("my-other-bundle", "one-source-file.js")]
        public void Resolve_ContextParam_ReturnsBundledOutput_WhenValidBundleIsFoundAndUseBundledOutputTrue(string bundleName, params string[] files)
        {
            string directory = "/my/output-directory";
            var outputFileName = $"{bundleName}_output.js";
            string expectedOutput = $"{directory}/{outputFileName}";

            var bundle = new Bundle(bundleName, "js", "/my/output-directory", files, outputFileName);
            var bundleLookup = new Mock<IBundleLookup>();
            bundleLookup.Setup(bl => bl.TryGetBundle(bundleName, out bundle)).Returns(true);

            var assetResolver = AssetResolverTestHelper.BuildTestResolver(bundleLookup.Object);

            var context = new AssetResolutionContext(new[] { bundleName }, useBundledOutput: true, includeVersioning: false);

            var assets = assetResolver.Resolve(context);

            Assert.Single(assets);
            Assert.Equal(expectedOutput, assets.First().Value);
        }

        [Fact]
        public void Resolve_ContextParam_RemovesDuplicateFiles_WhenBundleFilesHaveDuplicates()
        {
            string outputDirectory = "/my/output";
            string duplicateFile = "duplicate.js";
            var bundles = new List<Bundle>()
            {
                new Bundle("bundle-one", "js", outputDirectory, new string[] { "my-unique-file.js", duplicateFile }),
                new Bundle("bundle-two", "js", outputDirectory, new string[] { "other-unique-file.js", "another-unique.js", duplicateFile })
            };

            var bundleLookup = new Mock<IBundleLookup>();
            foreach (var bundle in bundles)
            {
                Bundle bundleCopy = bundle;
                bundleLookup.Setup(bl => bl.TryGetBundle(bundle.Name, out bundleCopy)).Returns(true);
            }

            var assetResolver = AssetResolverTestHelper.BuildTestResolver(bundleLookup.Object);

            var context = new AssetResolutionContext(bundles.Select(b => b.Name), useBundledOutput: false, includeVersioning: false);

            var assets = assetResolver.Resolve(context);

            Assert.Equal(4, assets.Count());
            Assert.Equal(1, assets.Count(a => a.Value == duplicateFile));
        }

        [Fact]
        public void Resolve_PathsParam_ReturnsEmpty_WhenBundlePathsIsNullOrEmpty()
        {
            var assetResolver = AssetResolverTestHelper.BuildTestResolver();
            Assert.Equal(Enumerable.Empty<string>(), assetResolver.Resolve(new string[] { }));
        }

        [Theory]
        [InlineData("my-bundle", "/my/source.js")]
        [InlineData("my-bundle", "/my/source.js", "some-other-bundle", "another-bundle")]
        public void Resolve_PathsParam_ReturnsSameAssetsAsContextParam_WhenBundlePathsSupplied(params string[] pathsOrBundles)
        {
            var settings = new BunderSettings();
            var assetResolver = AssetResolverTestHelper.BuildTestResolver(settings: settings);
            var assets = assetResolver.Resolve(new AssetResolutionContext(pathsOrBundles,
                                                                          useBundledOutput: settings.UseBundledOutput,
                                                                          includeVersioning: settings.UseVersioning));
            var paths = assetResolver.Resolve(pathsOrBundles);

            Assert.Equal(assets.Count(), paths.Count());
            Assert.Equal(assets.Select(a => a.Value), paths);
        }
    }
}
