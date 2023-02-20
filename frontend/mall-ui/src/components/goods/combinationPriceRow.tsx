import XCombinationPrice from './price';
import { Typography } from '@mui/material';
import Box from '@mui/material/Box';

export default function Demo(props: any) {
  const { model } = props;

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'flex-end',
      }}
      component="div"
    >
      <Box
        sx={{
          width: '100%',
        }}
      >
        <Typography color="text.secondary" variant="caption">
          {model.Name}
        </Typography>
      </Box>
      <Box
        sx={{
          display: 'inline-block',
          ml: 1,
        }}
      >
        <XCombinationPrice model={model} />
      </Box>
    </Box>
  );
}
