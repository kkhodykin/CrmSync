﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CrmSync.Tests
{
    [TestFixture]
    public class CrmSyncPluginTests
    {

        public CrmSyncPluginTests()
        {

        }

        [Test]
        public void Should_Set_Custom_Row_Version_Attribute()
        {
            var sut = new TestCrmSyncChangeTrackerPlugin();
            sut.Execute(null);

            var updatedEntity = sut.MockOrgService.CapturedInput.UpdateEntity;

            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.Attributes, Is.Not.Null);
            Assert.That(updatedEntity.Attributes.ContainsKey(CrmSyncChangeTrackerPlugin.CreatedRowVersionAttributeName));
            Assert.That(updatedEntity.Attributes.ContainsKey(CrmSyncChangeTrackerPlugin.RowVersionAttributeName));


            var rowVersion = (long)updatedEntity.Attributes[CrmSyncChangeTrackerPlugin.RowVersionAttributeName];
            Assert.That(rowVersion, Is.GreaterThan(0));

            var capturedCreationVersion = (long)updatedEntity.Attributes[CrmSyncChangeTrackerPlugin.CreatedRowVersionAttributeName];

            Assert.That(rowVersion, Is.EqualTo(capturedCreationVersion));

        }

    }

    /// <summary>
    /// Subject Under Test - we use extract and override technique.
    /// </summary>
    public class TestCrmSyncChangeTrackerPlugin : CrmSyncChangeTrackerPlugin
    {

        public TestCrmSyncChangeTrackerPlugin()
        {
            MockOrgService = new MockOrgService();
        }

        public MockOrgService MockOrgService { get; set; }

        /// <summary>
        /// Extract and override technique allows us to provide dependency during test.
        /// </summary>
        /// <returns></returns>
        protected override Microsoft.Xrm.Sdk.Entity EnsureTargetEntity()
        {
            var testEntity = new Entity("unittestentity");
            testEntity.Id = Guid.NewGuid();
            testEntity[CrmSyncChangeTrackerPlugin.RowVersionAttributeName] = 102525478L;
            return testEntity;
        }

        protected override void LoadServices(IServiceProvider serviceProvider)
        {
            //base.LoadServices(serviceProvider);
            // setup mock org service here.

        }

        protected override void EnsureTransaction()
        {
            //do nothing.
        }

        protected override IOrganizationService GetOrganisationService()
        {
            // Need to return fake org service and monitor its update method.
            return MockOrgService;
        }

    }
}