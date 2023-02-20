import u from '@/utils';
import { ChevronLeft } from '@mui/icons-material';
import { IconButton, Stack } from '@mui/material';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { history } from 'umi';
import XContainer from '../container';

export default function ButtonAppBar(props: {
  children?: any;
  route?: any;
  window?: any;
  //position?: string;
  nofixed?: boolean;
  leftButtons?: Array<any>;
  rightButtons?: Array<any>;
}) {
  const { children, route, rightButtons, leftButtons } = props;

  return (
    <XContainer>
      <Box sx={{}}>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            pt: 2,
            pb: 2,
          }}
        >
          <Stack
            direction="row"
            alignItems="center"
            spacing={{ xs: 0.5, sm: 1.5 }}
          >
            {u.isEmpty(leftButtons) && (
              <Box
                sx={{
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  cursor: 'pointer',
                }}
                onClick={() => {
                  if (history.length > 1) {
                    history.goBack();
                  } else {
                    history.push({
                      pathname: '/',
                    });
                  }
                }}
              >
                <IconButton size="large" color="inherit">
                  <ChevronLeft />
                </IconButton>
                <Typography variant="h2" sx={{}}>
                  {route.title || u.config.app.name}
                </Typography>
              </Box>
            )}
            {u.isEmpty(leftButtons) || u.map(leftButtons, (x) => x)}
          </Stack>
          <Stack
            direction="row"
            alignItems="center"
            spacing={{ xs: 0.5, sm: 1.5 }}
          >
            {u.isEmpty(rightButtons) || u.map(rightButtons, (x) => x)}
          </Stack>
        </Box>

        <Box sx={{ mt: 2 }}>{children}</Box>
      </Box>
    </XContainer>
  );
}
