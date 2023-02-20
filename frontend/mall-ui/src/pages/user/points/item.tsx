import { Box, Divider, Chip, Typography } from '@mui/material';
import u from '@/utils';

export default (props: any) => {
  const { model } = props;

  if (!model || model.Points == 0) {
    return null;
  }

  const renderPoints = () => {
    var points = model.Points;
    var action = model.ActionType;
    if (action == 1) {
      return (
        <Chip
          color="error"
          label={`+${points}`}
          variant="outlined"
          size="small"
        ></Chip>
      );
    } else if (action == -1) {
      return (
        <Chip
          color="success"
          label={`-${points}`}
          variant="outlined"
          size="small"
        ></Chip>
      );
    } else {
      return <Chip label={points} variant="outlined" size="small"></Chip>;
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
          <Box>{renderPoints()}</Box>
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
