import { GoodsDto } from '@/utils/models';
import { Box, Typography } from '@mui/material';
import XAdminComment from './adminComment';
import XCombinations from '../../combinations';
import XPicture from '../../picture';
import XWrapTags from './tags';
import XLabels from './labels';

export interface XGoodsBoxProp {
  model: GoodsDto;
  count?: number;
  lazy?: boolean;
}

const index = function (props: { data: XGoodsBoxProp }) {
  const { model, count, lazy } = props.data;

  if (!model || !model.Id) {
    return null;
  }

  return (
    <>
      <Box sx={{}}>
        <Box>
          <XWrapTags model={model}>
            <XPicture model={model} lazy={lazy == undefined ? true : lazy} />
          </XWrapTags>
        </Box>
        <Box
          sx={{
            padding: {
              xs: 1,
              sm: 1,
              md: 2,
            },
          }}
        >
          <Typography
            variant="subtitle2"
            component="div"
            sx={{
              fontWeight: 'bolder',
            }}
          >
            {model.Name || '--'}
          </Typography>

          <XAdminComment model={model} />
          <XCombinations model={model} count={count} />
          <XLabels model={model} />
        </Box>
      </Box>
    </>
  );
};

export default index;
