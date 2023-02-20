import { Box, Link, Typography } from '@mui/material';

export default function IndexPage() {
  return (
    <>
      <Box
        sx={{
          py: 4,
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <Link
          href="https://beian.miit.gov.cn/"
          target="_blank"
          underline="none"
          sx={{
            fontSize: 10,
          }}
        >
          <Typography variant="overline" color={'text.disabled'}>
            苏ICP备2022031079号-1
          </Typography>
        </Link>
      </Box>
    </>
  );
}
