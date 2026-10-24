
using ShogiBoard.Services;
namespace ShogiBoard.Tests.Services;

public class GameServiceTest
{
    private readonly string[,] boardData = new string[9,9];

    [Theory]
    [InlineData("l1n/+s1g/b1r", 3)]
    [InlineData("l1nK/+s2g/b1+Pr", 4)]
    [InlineData("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL", 9)]
    public void UpdateBoardDataTest_Positive(string text, int boardLength)
    {
        GameService service = new();
        service.UpdateBoardData(text, boardData, boardLength);

        string[] data = text.Split("/");
        for (int i = 0; i < data.Length; ++i)
        {
            string rank = data[i];
            int files = 0;
            for (int j = 0; j < rank.Length; ++j)
            {
                string s = rank.Substring(j, 1);
                if (s == "+")
                {
                    s = rank.Substring(j, 2);
                    Assert.Equal(s, boardData[i, files]);
                    j += 1;
                }
                else
                {
                    if (int.TryParse(s, out var n))
                    {
                        for (int k = 0; k < n; ++k)
                        {
                            Assert.Equal(string.Empty, boardData[i, files + k]);
                        }
                        files += n;
                        continue;
                    }
                    else
                    {
                        Assert.Equal(s, boardData[i, files]);
                    }
                }
                files += 1;
            }
        }
    }
}
