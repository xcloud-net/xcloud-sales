//this is a template of config structure
const data = {
    Condition: [
        {
            //match all orders
            ConditionType: "*",
            ConditionJson: JSON.stringify({
                let_this_json_empty: true
            })
        },
        {
            //only order total amount greater than 99
            ConditionType: "order-price",
            ConditionJson: JSON.stringify({
                LimitedOrderAmount: 99
            })
        }
    ],
    Result: [
        {
            //if condition meet , then will give you 9 yuan as promotion discount
            ResultType: "order-discount",
            ResultJson: JSON.stringify({
                DiscountAmount: 9
            })
        }
    ]
}

console.log(JSON.stringify(data));