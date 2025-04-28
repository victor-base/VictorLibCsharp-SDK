using System;
using Xunit;
using Victor;
using VictorBaseDotNET.Src.Common;


public class VictorSDKTests
{
    [Fact]
    public void Insert_ShouldInsertVectorSuccessfully()
    {
        // Arrange
        VictorSDK sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        ulong id = 1;
        float[] vector = new float[128];

        // Act
        int status = sdk.Insert(id, vector, 128);

        // Assert
        Assert.Equal(0, status); // Suponiendo que 0 es el código de éxito
    }


    [Fact]
    public void Search_ShouldReturnValidResult()
    {
        // Arrange
        VictorSDK sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        float[] vector = new float[128];

        // Act
        MatchResult result = sdk.Search(vector, 128);

        // Assert
        // Assert.True(result.MatchId >= 0);
        Assert.True(result.MatchData >= 0);
    }


    [Fact]
    public void GetSize_ShouldReturnCorrectSize()
    {
        // Arrange
        VictorSDK sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        sdk.Insert(1, new float[128], 128);

        // Act
        ulong size = sdk.GetSize();

        // Assert
        Assert.Equal((ulong)1, size);
    }


    [Fact]
    public void DumpIndex_ShouldNotThrowException()
    {
        // Arrange
        VictorSDK sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        string filename = "index_dump.dat";

        // Act & Assert
        var exception = Record.Exception(() => sdk.DumpIndex(filename));
        Assert.Null(exception);
    }


    [Fact]
    public void Contains_ShouldReturnTrueForExistingId()
    {
        // Arrange
        VictorSDK sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        ulong id = 1;
        sdk.Insert(id, new float[128], 128);

        // Act
        bool exists = sdk.Contains(id);

        // Assert
        Assert.True(exists);
    }
}