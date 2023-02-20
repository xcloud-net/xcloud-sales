import u from '@/utils';
import { Box, Typography, Button } from '@mui/material';
import { history } from 'umi';

export default function PinnedSubheaderList({ model }: { model: any }) {
  const data = model.Brands || [];

  if (u.isEmpty(data)) {
    return null;
  }

  return (
    <>
      <Box sx={{ px: 2, py: 1 }}>
        <Typography variant="overline" color="text.disabled" gutterBottom>
          热门品牌
        </Typography>
        <Box sx={{ mt: 1 }}>
          {u.map(data, (x, index) => {
            return (
              <Button
                key={index}
                size="small"
                variant="text"
                sx={{
                  color: (theme) => theme.palette.text.primary,
                }}
                onClick={() => {
                  history.push({
                    pathname: '/goods',
                    query: {
                      brand: x.Id,
                    },
                  });
                }}
              >
                {`${x.Name}`}
              </Button>
            );
          })}
        </Box>
      </Box>
    </>
  );
}
