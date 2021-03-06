﻿using DTORepository;
using DTORepository.Core;
using DTORepositoryTest.Samples.Dtos;
using DTORepositoryTest.Samples.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DTORepositoryTest
{
    public class DTORepositoryContainterTest : UnitTestBase
    {
        RepositoryFactory<BloggingContext> repositoryFactory;
        IRepository<BloggingContext, Blog> blogRepository;
        public DTORepositoryContainterTest(ITestOutputHelper output):base(output)
        {
            var mockContext = new Mock<BloggingContext>(_connection) { CallBase = true };
            mockContext.Setup(x => x.SaveChanges()).Throws(new InvalidProgramException("Mock Exception"));
            this.repositoryFactory = new RepositoryFactory<BloggingContext>(mockContext.Object);
            this.blogRepository = repositoryFactory.CreateRepository<Blog>();

        }
        [Fact]
        public void Test_Flag_ThrowsOnError_True()
        {
            var oldFlag = DTORepositoryContainer.ThrowsOnDatabaseError;
            try
            {
                Assert.Throws(typeof(InvalidProgramException), () =>
                {
                    DTORepositoryContainer.ThrowsOnDatabaseError = true;
                    this.blogRepository.Create(new BlogDto());
                });
            }
            finally
            {
                DTORepositoryContainer.ThrowsOnDatabaseError = oldFlag;
            }
        }
        [Fact]
        public void Test_Flag_ThrowsOnError_False()
        {
            var oldFlag = DTORepositoryContainer.ThrowsOnDatabaseError;
            try
            {
                DTORepositoryContainer.ThrowsOnDatabaseError = false;
                var status = this.blogRepository.Create(new BlogDto());
                Assert.Equal("Mock Exception", status.Errors[0].ErrorMessage);
            }finally
            {
                DTORepositoryContainer.ThrowsOnDatabaseError = oldFlag;
            }
        }
    }
}
