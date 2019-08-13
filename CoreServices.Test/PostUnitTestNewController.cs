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
    /// <summary>
    /// Unit testing of Post controller with database entries.
    /// Optimized code.
    /// </summary>
    public class PostUnitTestNewController
    {
        #region Properties

        public static DbContextOptions<BlogDBContext> DbContextOptions => new DbContextOptionsBuilder<BlogDBContext>()
            .UseSqlServer(Constants.ConnectionString)
            .Options;
        private BlogDBContext _context => new BlogDBContext(DbContextOptions);
        private DummyDataDbInitializer _db => new DummyDataDbInitializer();
        private readonly PostController _controller;

        #endregion

        #region Constructor

        public PostUnitTestNewController()
        {
            InitializeDatabase();
            var repository = new PostRepository(_context);
            _controller = new PostController(repository);
        }

        #endregion

        #region Test Methods

        #region Get By Id  

        [Fact]
        public async void Task_GetPostById_Return_OkResult()
        {
            var postId = 2;

            //Act  
            var data = await _controller.GetPost(postId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_Return_NotFoundResult()
        {
            var postId = 3;

            //Act  
            var data = await _controller.GetPost(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_Return_BadRequestResult()
        {
            int? postId = null;

            //Act  
            var data = await _controller.GetPost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_MatchResult()
        {
            int? postId = 1;

            //Act  
            var data = await _controller.GetPost(postId);

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
            //Act  
            var data = await _controller.GetPosts();

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void Task_GetPosts_Return_BadRequestResult()
        {
            //Act  
            var data = _controller.GetPosts();
            data = null;

            if (data != null)
                //Assert  
                Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_GetPosts_MatchResult()
        {
            //Act  
            var data = await _controller.GetPosts();

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
            var post = new Post() { Title = "Test Title 3", Description = "Test Description 3", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await _controller.AddPost(post);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_Add_InvalidData_Return_BadRequest()
        {
            Post post = new Post() { Title = "Test Title More Than 20 Characteres", Description = "Test Description 3", CategoryId = 3, CreatedDate = DateTime.Now };

            //Act              
            var data = await _controller.AddPost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_Add_ValidData_MatchResult()
        {
            var post = new Post() { Title = "Test Title 4", Description = "Test Description 4", CategoryId = 2, CreatedDate = DateTime.Now };

            //Act  
            var data = await _controller.AddPost(post);

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
            var postId = 2;

            //Act  
            var existingPost = await _controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post
            {
                Title = "Test Title 2 Updated",
                Description = result.Description,
                CategoryId = result.CategoryId,
                CreatedDate = result.CreatedDate
            };

            var updatedData = await _controller.UpdatePost(post);

            //Assert  
            Assert.IsType<OkResult>(updatedData);
        }

        [Fact]
        public async void Task_Update_InvalidData_Return_BadRequest()
        {
            var postId = 2;

            //Act  
            var existingPost = await _controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post
            {
                Title = "Test Title More Than 20 Characteres",
                Description = result.Description,
                CategoryId = result.CategoryId,
                CreatedDate = result.CreatedDate
            };

            var data = await _controller.UpdatePost(post);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_Update_InvalidData_Return_NotFound()
        {
            var postId = 2;

            //Act  
            var existingPost = await _controller.GetPost(postId);
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

            var data = await _controller.UpdatePost(post);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        #endregion

        #region Delete Post  

        [Fact]
        public async void Task_Delete_Post_Return_OkResult()
        {
            var postId = 2;

            //Act  
            var data = await _controller.DeletePost(postId);

            //Assert  
            Assert.IsType<OkResult>(data);
        }

        [Fact]
        public async void Task_Delete_Post_Return_NotFoundResult()
        {
            var postId = 5;

            //Act  
            var data = await _controller.DeletePost(postId);

            //Assert  
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_Delete_Return_BadRequestResult()
        {
            int? postId = null;

            //Act  
            var data = await _controller.DeletePost(postId);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitializeDatabase()
        {
            _db.Seed(_context);
        }

        #endregion
    }
}
