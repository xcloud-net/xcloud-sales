import { AfterSalesCommentDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';

export default ({ model }: { model: AfterSalesCommentDto }) => {
  return <>
    <Box sx={{
      my: 1,
    }}>
      <Typography variant={'overline'} color={'primary'} component={'div'}
                  gutterBottom>{model.IsAdmin ? `卖家：` : `你：`}</Typography>

      <Typography variant='body2' component={'div'}>{model.Content || '--'}</Typography>
      {
        //image or empty
      }
    </Box>
  </>;
};
