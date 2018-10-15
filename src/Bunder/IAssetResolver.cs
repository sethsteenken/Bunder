﻿using System.Collections.Generic;

namespace Bunder
{
    /// <summary>
    /// Resolve values to a list of <see cref="Asset"/> based on list of paths or bundles and other various settings defined in <see cref="AssetResolutionContext"/>.
    /// </summary>
    public interface IAssetResolver
    {
        /// <summary>
        /// Resolves values defined in <paramref name="context"/> to a list of renderable <see cref="Asset"/>.
        /// Paths or bundles defined at <see cref="AssetResolutionContext.PathsOrBundles"/> will be resolved to a list of <see cref="Bundle"/> and then to a list of <see cref="Asset"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IReadOnlyList<Asset> Resolve(AssetResolutionContext context);
    }
}
