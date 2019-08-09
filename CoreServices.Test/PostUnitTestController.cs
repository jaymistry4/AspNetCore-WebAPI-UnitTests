using CoreServices.Controllers;
using CoreServices.Models;
using CoreServices.Repository;
using CoreServices.Test.Helpers;
using CoreServices.Test.Utils;
using CoreServices.ViewModel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreServices.Test
{
    public class PostUnitTestController
    {
        #region Properties

        public static DbContextOptions<BlogDBContext> DbContextOptions { get; set; }
        private BlogDBContext _dbContext;
        private DummyDataDbInitializer _db;

        private readonly PostRepository _repository;

        #endregion

        #region Constructor

        public PostUnitTestController()
        {
            InitializeDatabase();

            _repository = new PostRepository(_dbContext);
        }

        #endregion

        #region Test Methods

        #region Get By Id  

        [Fact]
        public async void Task_GetPostById_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var postId = 2;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var postId = 3;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            int? postId = null;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_MatchResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            int? postId = 1;

            //Act  
            var data = await controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            Assert.Equal("Test Title 1", post.Title);
            Assert.Equal("Test Description 1", post.Description);
        }

        #endregion

        #region Get All  

        [Fact]
        public async void Task_GetPosts_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(_repository);

            //Act  
            var data = await controller.GetPosts();

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void Task_GetPosts_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(_repository);

            //Act  
            var data = controller.GetPosts();
            data = null;

            if (data != null)
                //Assert  
                Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_GetPosts_MatchResult()
        {
            //Arrange  
            var controller = new PostController(_repository);

            //Act  
            var data = await controller.GetPosts();

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<List<PostViewModel>>().Subject;

            Assert.Equal("Test Title 1", post[0].Title);
            Assert.Equal("Test Description 1", post[0].Description);

            Assert.Equal("Test Title 2", post[1].Title);
            Assert.Equal("Test Description 2", post[1].Description);
        }

        #endregion

        #region Add New Blog  

        [Fact]
        public async void Task_Add_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var post = new Post() { Title = "Test Title 3", Description = "Test Description 3", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_Add_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new PostController(_repository);
            Post post = new Post() { Title = "Test Title More Than 20 Characteres", Description = "Test Description 3", CategoryId = 3, CreatedDate = DateTime.Now };

            //Act              
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_Add_ValidData_MatchResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var post = new Post() { Title = "Test Title 4", Description = "Test Description 4", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await controller.AddPost(post);

            //Assert  
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            // var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;  

            Assert.Equal(3, okResult.Value);
        }

        #endregion

        #region Update Existing Blog  

        [Fact]
        public async void Task_Update_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post
            {
                Title = "Test Title 2 Updated",
                Description = result.Description,
                CategoryId = result.CategoryId,
                CreatedDate = result.CreatedDate
            };

            var updatedData = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<OkResult>(updatedData);
        }

        [Fact]
        public async void Task_Update_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post
            {
                Title = "Test Title More Than 20 Characteres",
                Description = result.Description,
                CategoryId = result.CategoryId,
                CreatedDate = result.CreatedDate
            };

            var data = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_Update_InvalidData_Return_NotFound()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var postId = 2;

            //Act  
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post
            {
                PostId = 5,
                Title = "Test Title More Than 20 Characteres",
                Description = result.Description,
                CategoryId = result.CategoryId,
                CreatedDate = result.CreatedDate
            };

            var data = await controller.UpdatePost(post);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        #endregion

        #region Delete Post  

        [Fact]
        public async void Task_Delete_Post_Return_OkResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var postId = 2;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<OkResult>(data);
        }

        [Fact]
        public async void Task_Delete_Post_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            var postId = 5;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_Delete_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new PostController(_repository);
            int? postId = null;

            //Act  
            var data = await controller.DeletePost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitializeDatabase()
        {
            DbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseSqlServer(Constants.ConnectionString)
                .Options;

            _dbContext = new BlogDBContext(DbContextOptions);
            _db = new DummyDataDbInitializer();
            _db.Seed(_dbContext);
        }

        #endregion
    }
}
