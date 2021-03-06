#region Using Directives

using System;
using System.IO;
using System.IO.IsolatedStorage;
using Innovatian.Configuration.Tests.Classes;
using Xunit;

#endregion

namespace Innovatian.Configuration.Tests
{
    // These tests are all integration tests.
    public class IsoStorageConfigurationSourceTests
    {
        private const IsolatedStorageScope Scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;
        private readonly string _fileName = typeof (IsoStorageConfigurationSourceTests).Name + ".xml";

        [Fact]
        public void EmptyFileNameThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => new IsoStorageConfigurationSource( Scope, string.Empty ) );
        }

        [Fact]
        public void NullFileNameThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => new IsoStorageConfigurationSource( Scope, null ) );
        }

        /* 
         * System.IO.IsolatedStorage.IsolatedStorageException : Unable to determine application identity of the caller.
         * Need to find a way to test application scope IsoStorage in unit testing.
         * 
        [Fact]
        public void CanLoadAndSaveWithDefaultScope()
        {
            var source = new IsoStorageConfigurationSource( _fileName );
            RunCreationTest( source );
        }
        
        [Fact]
        public void CanLoadAndSaveInApplicationMachineScope()
        {
            var source = new IsoStorageConfigurationSource( IsolatedStorageScope.Application | IsolatedStorageScope.Machine, _fileName );
            RunCreationTest( source );
        }
        
        [Fact]
        public void CanLoadAndSaveInApplicationUserScope()
        {
            var source = new IsoStorageConfigurationSource(IsolatedStorageScope.Application | IsolatedStorageScope.User, _fileName);
            RunCreationTest(source);
        }
        */

        [Fact]
        public void CanLoadAndSaveInMachineAssemblyScope()
        {
            const IsolatedStorageScope scope = IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly;
            var source = new IsoStorageConfigurationSource( scope, _fileName );
            RunCreationTest( source );
        }

        [Fact]
        public void CanLoadAndSaveInAssemblyUserScope()
        {
            const IsolatedStorageScope scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;
            var source = new IsoStorageConfigurationSource( scope, _fileName );
            RunCreationTest( source );
        }

        [Fact]
        public void CanLoadAndSaveInMachineStorForDomainScope()
        {
            const IsolatedStorageScope scope =
                IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain;
            var source =
                new IsoStorageConfigurationSource( scope, _fileName );
            RunCreationTest( source );
        }

        [Fact]
        public void CanLoadAndSaveInUserStorForDomainScope()
        {
            const IsolatedStorageScope scope =
                IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain;
            var source =
                new IsoStorageConfigurationSource( scope, _fileName );
            RunCreationTest( source );
        }

        [Fact]
        public void CreationWithNoneScopeThrows()
        {
            Assert.Throws<ArgumentException>(
                () => new IsoStorageConfigurationSource( IsolatedStorageScope.None, _fileName ) );
        }

        private void RunCreationTest( IsoStorageConfigurationSource source )
        {
            string sourceFile = string.Empty;
            try
            {
                source.Add( SectionGenerator.GetSingleSection() );
                source.Save();
                sourceFile = source.FullPath;
                // we should now have a file on the hdd with the settings we want.
                string sourceAsXml = XmlConfigurationSource.ToXml( source );

                // Now create a new instance so it can load the data.
                var newSource = new IsoStorageConfigurationSource( source.Scope, _fileName );
                string newSourceAsXml = XmlConfigurationSource.ToXml( newSource );
                Assert.Equal( sourceAsXml, newSourceAsXml );
            }
            finally
            {
                if ( File.Exists( sourceFile ) )
                {
                    File.Delete( sourceFile );
                }
            }
        }
    }
}