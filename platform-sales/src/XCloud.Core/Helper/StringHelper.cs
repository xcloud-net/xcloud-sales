namespace XCloud.Core.Helper;

/// <summary>
/// 字符串帮助类
/// </summary>
public static class StringHelper
{
    #region 计算匹配率/相似度
    /// <summary>
    /// 计算相似度。
    /// </summary>
    public static SimilarityResult SimilarityRate(string str1, string str2)
    {
        var result = new SimilarityResult();
        var arrChar1 = str1.ToCharArray();
        var arrChar2 = str2.ToCharArray();
        var computeTimes = 0;
        var row = arrChar1.Length + 1;
        var column = arrChar2.Length + 1;
        var matrix = new int[row, column];
        //初始化矩阵的第一行和第一列
        for (var i = 0; i < column; i++)
        {
            matrix[0, i] = i;
        }
        for (var i = 0; i < row; i++)
        {
            matrix[i, 0] = i;
        }
        for (var i = 1; i < row; i++)
        {
            for (var j = 1; j < column; j++)
            {
                var intCost = 0;
                intCost = arrChar1[i - 1] == arrChar2[j - 1] ? 0 : 1;
                //关键步骤，计算当前位置值为左边+1、上面+1、左上角+intCost中的最小值 
                //循环遍历到最后_Matrix[_Row - 1, _Column - 1]即为两个字符串的距离
                matrix[i, j] = new int[] { matrix[i - 1, j] + 1, matrix[i, j - 1] + 1, matrix[i - 1, j - 1] + intCost }.Min();
                computeTimes++;
            }
        }
        //相似率 移动次数小于最长的字符串长度的20%算同一题
        var intLength = row > column ? row : column;
        //_Result.Rate = (1 - (double)_Matrix[_Row - 1, _Column - 1] / intLength).ToString().Substring(0, 6);
        result.Rate = (1 - (double)matrix[row - 1, column - 1] / (intLength - 1));
        result.ComputeTimes = computeTimes.ToString() + " 距离为：" + matrix[row - 1, column - 1].ToString();
        return result;
    }

    /// <summary>
    /// 计算结果
    /// </summary>
    public struct SimilarityResult
    {
        /// <summary>
        /// 相似度，0.54即54%。
        /// </summary>
        public double Rate;
        /// <summary>
        /// 对比次数
        /// </summary>
        public string ComputeTimes;
    }
    #endregion
}