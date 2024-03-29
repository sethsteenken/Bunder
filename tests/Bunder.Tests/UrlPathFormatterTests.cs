﻿using Moq;
using System;
using Xunit;

namespace Bunder.Tests
{
    public class UrlPathFormatterTests
    {
        [Fact]
        public void GetFullPath_ThrowsException_WhenVirtualPathIsNull()
        {
            var formatter = UrlPathFormatterTestHelper.BuildFormatter();
            Assert.Throws<ArgumentNullException>(() => formatter.GetFullPath(null, includeVersioning: false));
        }

        [Theory]
        [InlineData("<>@#$@$^@#$%")]
        [InlineData(@"\some\windows\path.txt")]
        public void GetFullPath_ThrowsException_WhenVirtualPathIsNotValidUri(string invalidPath)
        {
            var formatter = UrlPathFormatterTestHelper.BuildFormatter();
            Assert.Throws<FormatException>(() => formatter.GetFullPath(invalidPath, includeVersioning: false));
        }

        [Theory]
        [InlineData("/valid/path.jpg")]
        [InlineData("valid-file.js")]
        [InlineData("/valid/path/")]
        [InlineData("/valid/path")]
        public void GetFullPath_ReturnsValidFullPathWithVersioning_WhenVirtualPathIsValid(string path)
        {
            const string baseUrl = "https://www.myawesomedomain.com/";
            const string versionIndicator = "?v=versioned";

            var versioningFormatter = new Mock<IVersioningFormatter>();
            versioningFormatter.Setup(v => v.GetVersionedPath(path)).Returns($"{path}{versionIndicator}");

            var formatter = UrlPathFormatterTestHelper.BuildFormatter(versioningFormatter: versioningFormatter.Object);

            var result = formatter.GetFullPath(path, includeVersioning: true);
            string expected = $"{path}{versionIndicator}";

            versioningFormatter.Verify(v => v.GetVersionedPath(It.IsAny<string>()), Times.Once);
            Assert.Equal(expected, result);
        }
    }
}
