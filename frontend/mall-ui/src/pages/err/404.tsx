import { Box, Typography } from '@mui/material';

export default function Types() {
  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
      }}
    >
      <Typography
        variant="h1"
        sx={{
          display: 'inline',
        }}
      >
        404.Not Found
      </Typography>
    </Box>
  );
}
