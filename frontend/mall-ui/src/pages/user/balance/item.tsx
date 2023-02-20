import { Box, Divider, Chip, Typography } from '@mui/material';
import u from '@/utils';

export default (props: any) => {
  const { model } = props;

  if (!model || model.Balance == 0) {
    return null;
  }

  const renderBalance = () => {
    var balance = model.Balance;
    var action = model.ActionType;
    if (action == 1) {
      return (
        <Chip
          color="error"
          label={`+${balance}`}
          variant="outlined"
          size="small"
        ></Chip>
      );
    } else if (action == -1) {
      return (
        <Chip
          color="success"
          label={`-${balance}`}
          variant="outlined"
          size="small"
        ></Chip>
      );
    } else {
      return <Chip label={balance} variant="outlined" size="small"></Chip>;
    }
  };

  return (
    <>
      <Box
        sx={{
          p: 2,
        }}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          <Box>{renderBalance()}</Box>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'end',
            }}
          >
            <Typography variant="button" component={'div'}>
              {u.dateTimeFromNow(model.CreationTime)}
            </Typography>
            {u.isEmpty(model.Message) || (
              <Box sx={{ py: 1 }}>
                <Typography
                  variant="overline"
                  color="primary"
                  component={'div'}
                >
                  {model.Message}
                </Typography>
              </Box>
            )}
          </Box>
        </Box>
      </Box>
      <Divider />
    </>
  );
};
