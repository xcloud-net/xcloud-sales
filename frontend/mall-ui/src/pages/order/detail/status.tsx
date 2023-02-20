import utils from '@/utils/order';
import { Box, Typography, StepContent } from '@mui/material';
import Step from '@mui/material/Step';
import StepLabel from '@mui/material/StepLabel';
import Stepper from '@mui/material/Stepper';
import u from '@/utils';
import { OrderDto } from '@/utils/models';

interface IEvent {
  name?: string;
  time?: string | null | undefined;
  desc?: string;
  completed?: boolean;
  active?: boolean;
  disabled?: boolean;
  error?: boolean;
  icon?: any;
  show?: boolean;
}

export default function HorizontalLabelPositionBelowStepper(props: {
  model: OrderDto;
}) {
  const { model } = props;
  const statusId = model.OrderStatusId || -100;

  const orderStatus = utils.getOrderStatus(model.OrderStatusId);
  const paymentStatus = utils.getOrderStatus(model.PaymentStatusId);

  const getEvents = () => {
    //支付节点
    var pending = model.OrderStatusId == utils.OrderStatus.Pending;
    var paid =
      paymentStatus && paymentStatus.status == utils.PaymentStatus.Paid;
    const paymentNode: IEvent = {
      name: paymentStatus?.name || '付款',
      active: pending && !paid,
      completed: paid || statusId > utils.OrderStatus.Pending,
    };
    //商家处理
    const processingNode: IEvent = {
      active: model.OrderStatusId == utils.OrderStatus.Processing,
      completed: statusId > utils.OrderStatus.Processing,
    };
    //发货
    const shippingNode: IEvent = {
      show: model.ShippingRequired || true,
      active: model.OrderStatusId == utils.OrderStatus.Delivering,
      completed: statusId > utils.OrderStatus.Delivering,
    };
    //完成
    const finishedNode: IEvent = {
      completed: model.OrderStatusId === utils.OrderStatus.Complete,
    };

    const events: IEvent[] = [
      {
        name: '下单',
        show: true,
        completed: true,
      },
      {
        name: '付款',
        show: true,
        ...paymentNode,
      },
      {
        name: '处理',
        show: true,
        ...processingNode,
      },
      {
        name: '配送',
        show: true,
        ...shippingNode,
      },
      {
        name: '完成',
        show: true,
        ...finishedNode,
      },
    ];

    console.log('订单进度', events);
    return events;
  };

  const events = getEvents();

  const renderStatus = () => {
    return (
      <>
        {orderStatus && (
          <Typography variant="h2" sx={{ display: 'inline' }}>
            {orderStatus.name}
          </Typography>
        )}
      </>
    );
  };

  return (
    <Box sx={{}}>
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          py: 5,
        }}
      >
        {renderStatus()}
      </Box>
      {model.OrderStatusId != utils.OrderStatus.Cancelled && (
        <Stepper activeStep={1} alternativeLabel>
          {u.map(
            events,
            (x, index) =>
              x.show && (
                <Step
                  key={index}
                  completed={x.completed}
                  active={x.active}
                  disabled={x.disabled}
                >
                  <StepLabel
                    optional={x.time}
                    StepIconComponent={x.icon}
                    error={x.error}
                  >
                    {x.name}
                  </StepLabel>
                  {x.desc && <StepContent>{x.desc}</StepContent>}
                </Step>
              ),
          )}
        </Stepper>
      )}
    </Box>
  );
}
