﻿namespace Bunder
{
    /// <summary>
    /// Model that represents a single item or asset with a <see cref="Asset.Value"/> representing the call to action or path.
    /// Assets represent each item that should be handled or rendered by Bunder.
    /// An Asset could be a reference to a <see cref="Bunder.Bundle"/> or represent a single file that is part of a bundle.
    /// </summary>
    public sealed class Asset
    {
        public Asset(string value, bool isStatic = false, Bundle? bundle = null)
        {
            Value = value;
            IsStatic = isStatic;
            Bundle = bundle;
        }

        /// <summary>
        /// Typically a path or call to action.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Asset Value is static and will not be compared against other bundles or bundle files.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Asset is referencing a <see cref="Bunder.Bundle"/>.
        /// </summary>
        public bool IsBundle => Bundle != null;

        /// <summary>
        /// The bundle reference if this Asset represents a bundle.
        /// </summary>
        public Bundle? Bundle { get; private set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
