using Moq;
using uiPolicyApi.Data.Entities;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.Implementation.Services;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.Implementation.Tests;

public class PolicyServiceTests
{
    [Fact]
    public async Task GetPolicyDetailsAsync_PolicyIdIsValid()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();

        var now = DateTime.UtcNow;
        
        mockPolicyRepository
            .Setup(m => m.GetPolicyAsync(12345))
            .ReturnsAsync(new PolicyEntity
            {
                Id = 12345,
                StartDate = DateOnly.FromDateTime(now),
                EndDate = DateOnly.FromDateTime(now.AddYears(1)),
                Amount = 1000m,
                AutoRenew = true
            });

        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.GetPolicyDetailsAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal((uint)12345, result.Result.Id);
    }
    
    [Fact]
    public async Task GetPolicyDetailsAsync_PolicyNotFound()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        mockPolicyRepository
            .Setup(m => m.GetPolicyAsync(12345))
            .ReturnsAsync(default(PolicyEntity));

        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.GetPolicyDetailsAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreatePolicyAsync_StartDateInThePast()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreatePolicyAsync_StartDateLaterThan60Days()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)),
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task CreatePolicyAsync_PolicyLengthMustBeOneYearV1()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = startDate,
            EndDate = startDate.AddMonths(6) // incorrect end date
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task CreatePolicyAsync_PolicyLengthMustBeOneYearV2()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = startDate,
            EndDate = startDate.AddMonths(18) // incorrect end date
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task CreatePolicyAsync_PolicyLengthMustBeOneYearV3()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        mockPolicyRepository.Setup(m=>m.AddPolicyAsync(It.IsAny<PolicyEntity>()))
            .ReturnsAsync(new PolicyEntity { Id = 12345 });
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345))
            .ReturnsAsync(new PolicyEntity { Id = 12345 });
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = startDate,
            EndDate = startDate.AddMonths(12).AddDays(-1),
            PolicyHolders = new List<SDK.Models.Policy.PolicyHolderModel>
            {
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-21))
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18))
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-21))
                }
            }
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
    }
    
    [Fact]
    public async Task CreatePolicyAsync_PolicyHoldersMustBeAtLeast16YearsOldV1()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        mockPolicyRepository.Setup(m=>m.AddPolicyAsync(It.IsAny<PolicyEntity>()))
            .ReturnsAsync(new PolicyEntity { Id = 12345 });
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345))
            .ReturnsAsync(new PolicyEntity { Id = 12345 });
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = startDate,
            EndDate = startDate.AddMonths(12).AddDays(-1),
            PolicyHolders = new List<SDK.Models.Policy.PolicyHolderModel>
            {
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-12)) // under 16
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18))
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-21))
                }
            }
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task CreatePolicyAsync_PolicyHoldersMustBeAtLeast16YearsOldV2()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        mockPolicyRepository.Setup(m=>m.AddPolicyAsync(It.IsAny<PolicyEntity>()))
            .ReturnsAsync(new PolicyEntity { Id = 12345 });
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345))
            .ReturnsAsync(new PolicyEntity { Id = 12345 });
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = startDate,
            EndDate = startDate.AddMonths(12).AddDays(-1),
            PolicyHolders = new List<SDK.Models.Policy.PolicyHolderModel>
            {
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25))
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18))
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-21))
                }
            }
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
    }
    
    [Fact]
    public async Task CreatePolicyAsync_ValidPolicyIsCreated()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        mockPolicyRepository.Setup(m=>m.AddPolicyAsync(It.IsAny<PolicyEntity>()))
            .ReturnsAsync(new PolicyEntity { Id = 12345 })
            .Verifiable(Times.Once());
        mockPolicyRepository.Setup(m=>m.AddPropertyToPolicyAsync(12345, It.IsAny<PolicyPropertyEntity>()))
            .ReturnsAsync(new PolicyEntity { Id = 12345 })
            .Verifiable(Times.Once());
        mockPolicyRepository.Setup(m=>m.AddHolderToPolicyAsync(12345, It.IsAny<PolicyHolderEntity>()))
            .ReturnsAsync(new PolicyEntity { Id = 12345 })
            .Verifiable(Times.Exactly(3));
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345))
            .ReturnsAsync(new PolicyEntity { Id = 12345 });
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);
        
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        
        var result = await policyService.CreatePolicyAsync(new QuoteModel
        {
            StartDate = startDate,
            EndDate = startDate.AddMonths(12).AddDays(-1),
            PolicyHolders = new List<SDK.Models.Policy.PolicyHolderModel>
            {
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25))
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18))
                },
                new()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-21))
                }
            }
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        mockPolicyRepository.Verify();
    }

    [Fact]
    public async Task RenewPolicyAsync_InvalidPolicy()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345)).ReturnsAsync(default(PolicyEntity));
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.RenewPolicyAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task RenewPolicyAsync_AlreadyExpired()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        var now = DateTime.UtcNow;
        
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345)).ReturnsAsync(new PolicyEntity
        {
            Id = 12345,
            StartDate = DateOnly.FromDateTime(now.AddYears(-2)),
            EndDate = DateOnly.FromDateTime(now.AddYears(-1))
        });
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.RenewPolicyAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task RenewPolicyAsync_MoreThan30Days()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        var now = DateTime.UtcNow;
        var expiredDate = now.AddDays(31);
        
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345)).ReturnsAsync(new PolicyEntity
        {
            Id = 12345,
            StartDate = DateOnly.FromDateTime(expiredDate.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(expiredDate)
        });
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.RenewPolicyAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task RenewPolicyAsync_ValidRenewalCreatesNewPolicy()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        var now = DateTime.UtcNow;
        var expiredDate = now.AddDays(15);
        
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345)).ReturnsAsync(new PolicyEntity
        {
            Id = 12345,
            StartDate = DateOnly.FromDateTime(expiredDate.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(expiredDate),
            AutoRenew = true
        });

        mockPolicyRepository.Setup(m => m.AddPolicyAsync(It.IsAny<PolicyEntity>()))
            .ReturnsAsync(new PolicyEntity
            {
                Id = 12347,
                StartDate = DateOnly.FromDateTime(expiredDate.AddYears(-1)),
                EndDate = DateOnly.FromDateTime(expiredDate)
            })
            .Verifiable(Times.Once());

        mockPaymentRepository.Setup(m => m.AddPaymentToPolicyAsync(12347, It.IsAny<PaymentEntity>()))
            .ReturnsAsync(new PaymentEntity
            {
                Id = 1,
                Amount = 1000m,
            })
            .Verifiable(Times.Once());
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.RenewPolicyAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal((uint)12347, result.Result.PolicyId);
        mockPolicyRepository.Verify();
        mockPaymentRepository.Verify();
    }

    [Fact]
    public async Task RenewPolicyAsync_ValidRenewalCreatesNewPolicyNoAutoRenew()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        var now = DateTime.UtcNow;
        var expiredDate = now.AddDays(15);
        
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345)).ReturnsAsync(new PolicyEntity
        {
            Id = 12345,
            StartDate = DateOnly.FromDateTime(expiredDate.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(expiredDate),
            AutoRenew = false
        });

        mockPolicyRepository.Setup(m => m.AddPolicyAsync(It.IsAny<PolicyEntity>()))
            .ReturnsAsync(new PolicyEntity
            {
                Id = 12347,
                StartDate = DateOnly.FromDateTime(expiredDate.AddYears(-1)),
                EndDate = DateOnly.FromDateTime(expiredDate),
                AutoRenew = false
            })
            .Verifiable(Times.Once());

        mockPaymentRepository.Setup(m => m.AddPaymentToPolicyAsync(12347, It.IsAny<PaymentEntity>()))
            .ReturnsAsync(new PaymentEntity
            {
                Id = 1,
                Amount = 1000m,
            })
            .Verifiable(Times.Never());
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.RenewPolicyAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal((uint)12347, result.Result.PolicyId);
        mockPolicyRepository.Verify();
        mockPaymentRepository.Verify();
    }
    
    [Fact]
    public async Task CancelPolicyAsync_InvalidPolicy()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345)).ReturnsAsync(default(PolicyEntity));
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.CancelPolicyAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task CancelPolicyAsync_PolicyAlreadyExpired()
    {
        // Arrange
        var mockPolicyRepository = new Mock<IPolicyRepository>();
        var mockPaymentRepository = new Mock<IPaymentRepository>();
        
        var now = DateTime.UtcNow;
        
        mockPolicyRepository.Setup(m=>m.GetPolicyAsync(12345)).ReturnsAsync(new PolicyEntity
        {
            StartDate = DateOnly.FromDateTime(now.AddYears(-2)),
            EndDate = DateOnly.FromDateTime(now.AddYears(-1))
        });
        
        // Act
        var policyService = new PolicyService(mockPolicyRepository.Object, mockPaymentRepository.Object);

        var result = await policyService.CancelPolicyAsync(12345);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
    }
}