namespace XCloud.Sales.Service.Users
{
    public static class UsersExtension
    {
        public static async Task ChangeUserBalanceAsync(this IUserBalanceService userBalanceService,
            int userId, decimal amount, BalanceActionType actionType, string comment = null)
        {
            if (userId <= 0)
                throw new ArgumentNullException(nameof(userId));

            if (decimal.Equals(amount, decimal.Zero))
                throw new ArgumentNullException(nameof(amount));

            var history = new BalanceHistoryDto(userId, amount, actionType, comment);

            await userBalanceService.UpdateUserBalanceAsync(history);
        }
    }
}