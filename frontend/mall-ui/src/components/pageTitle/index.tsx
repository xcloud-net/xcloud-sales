import { Box, Card, CardContent, Stack, Typography } from '@mui/material';

const ResponsiveAppBar = (props: any) => {
  const { title, desc, right } = props;
  return (
    <>
      <Card
        sx={{
          marginBottom: '10px',
        }}
      >
        <CardContent>
          <Stack
            direction={'row'}
            alignItems="center"
            justifyContent={'space-between'}
          >
            <Box>
              <Typography variant="h4">{title}</Typography>

              <Typography
                variant="overline"
                display="block"
                color="primary"
                gutterBottom
              >
                {desc}
              </Typography>
            </Box>
            <Box>{right}</Box>
          </Stack>
        </CardContent>
      </Card>
    </>
  );
};
export default ResponsiveAppBar;
