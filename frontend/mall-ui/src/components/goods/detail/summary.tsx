import u from '@/utils';
import { Box, Card, CardContent, Typography } from '@mui/material';

export default (props: any) => {
  const { model } = props;

  return (
    <>
      <Box sx={{}}>
        <Card
          sx={{
            marginBottom: '10px',
            borderRadius: 0,
          }}
        >
          <CardContent>
            <Typography variant="h5">{model.Name}</Typography>
            {model.AdminComment && (
              <Box sx={{}}>
                <Typography variant="overline" color="primary" display="block">
                  {`${model.AdminComment}`}
                </Typography>
              </Box>
            )}
            {model.Brand && (
              <Box sx={{}}>
                <Typography variant="overline" color="primary">
                  {`品牌：${model.Brand.Name}`}
                </Typography>
              </Box>
            )}
            {model.Category && (
              <Box sx={{}}>
                <Typography variant="overline" color="primary">
                  {`类目：${model.Category.Name}`}
                </Typography>
              </Box>
            )}
            <Typography
              variant="button"
              sx={{
                fontWeight: 'lighter',
              }}
            >
              {model.ShortDescription}
            </Typography>
            {u.isEmpty(model.Tags) || (
              <Box sx={{ mt: 1 }}>
                {u.map(model.Tags, (x, index) => {
                  return (
                    <Typography
                      key={index}
                      variant="overline"
                      color="primary"
                      sx={{
                        display: 'inline',
                        mr: 1,
                        mt: 1,
                      }}
                    >
                      {`#${x.Name}`}
                    </Typography>
                  );
                })}
              </Box>
            )}
          </CardContent>
        </Card>
      </Box>
    </>
  );
};
