import u from '@/utils';
import { Box, Typography, Chip } from '@mui/material';
import { history } from 'umi';

export default function IconChips({ model }: { model: any }) {
  const data = model.Tags || [];

  if (u.isEmpty(data)) {
    return null;
  }

  return (
    <>
      <Box sx={{ px: 2, py: 1 }}>
        <Typography variant="overline" color="text.disabled" gutterBottom>
          标签
        </Typography>
        <Box sx={{ mt: 1 }}>
          {u.map(data, (x, index) => {
            return (
              <Chip
                key={index}
                //variant='outlined'
                size="small"
                sx={{ mr: 1, mb: 1 }}
                label={x.Name}
                onClick={() => {
                  history.push({
                    pathname: '/goods',
                    query: {
                      tag: x.Id,
                    },
                  });
                }}
              />
            );
          })}
        </Box>
      </Box>
    </>
  );
}
