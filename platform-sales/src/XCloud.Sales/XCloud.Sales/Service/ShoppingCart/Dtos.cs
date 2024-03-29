﻿using Volo.Abp.Application.Dtos;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.ShoppingCart;

public class RemoveCartBySpecs : IEntityDto
{
    public int UserId { get; set; }
    public int[] GoodsSpecCombinationId { get; set; }
}

public class AddShoppingCartInput : IEntityDto
{
    public int UserId { get; set; }
    public int GoodsSpecCombinationId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// 修改购物车子项
/// </summary>
public class UpdateCartModel : IEntityDto
{
    /// <summary>
    /// 购物车子项Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 子项数量
    /// </summary>
    public int Quantity { get; set; }
}

public class ShoppingCartItemDto : ShoppingCartItem, IEntityDto
{
    public GoodsDto Goods { get; set; }

    public GoodsSpecCombinationDto GoodsSpecCombination { get; set; }

    public string[] Waring { get; set; }
}