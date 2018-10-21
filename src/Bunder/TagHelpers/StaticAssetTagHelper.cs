﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bunder.TagHelpers
{
    /// <summary>
    /// Base tag helper for rendering one or more static asset values.
    /// </summary>
    public abstract class StaticAssetTagHelper : TagHelper
    {
        public const string AttributeNameAsset = "asset";

        protected StaticAssetTagHelper(BunderSettings settings, IAssetResolver assetResolver)
        {
            Settings = settings;
            AssetResolver = assetResolver;
        }

        protected BunderSettings Settings { get; private set; }
        protected IAssetResolver AssetResolver { get; private set; }

        [HtmlAttributeName(AttributeNameAsset)]
        public string Asset { get; set; }

        [HtmlAttributeName("use-bundled-output")]
        public bool? UseBundledOutput { get; set; }

        [HtmlAttributeName("use-versioning")]
        public bool? UseVersioning { get; set; }

        protected IReadOnlyList<string> Assets => Asset?.Replace('|', ',').Replace(';', ',').Split(',');

        protected abstract Task ProcessStaticAssetTagAsync(TagHelperContext context, TagHelperOutput output, IReadOnlyList<Asset> assets);

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var assets = AssetResolver.Resolve(new AssetResolutionContext(
                            Assets, 
                            useBundledOutput: UseBundledOutput ?? Settings.UseBundledOutput, 
                            includeVersioning: UseVersioning ?? Settings.UseVersioning));

            await ProcessStaticAssetTagAsync(context, output, assets);
        }
    }
}
