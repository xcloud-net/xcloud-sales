import { Stack, Typography } from '@mui/material';
import { InfiniteScroll } from 'antd-mobile';
import XLoading from '../loading/dots';

export default function VerticalTabs(props: {
  children: any;
  hasMore: boolean;
  loading: boolean;
  onLoad: any | Promise<void>;
}) {
  const { children, hasMore, loading, onLoad } = props;

  const triggerLoadMore = async () => {
    if (loading) {
      return;
    }
    console.log('触底加载更多');
    onLoad && (await onLoad());
  };

  return (
    <>
      {children}
      <InfiniteScroll
        loadMore={async () => {
          if (loading || !hasMore) {
            return;
          }
          await triggerLoadMore();
        }}
        hasMore={hasMore}
      >
        {hasMore && <XLoading />}
        {hasMore || (
          <Stack spacing={2} justifyContent="center" direction="row">
            <Typography
              variant="overline"
              sx={{
                color: 'gray',
                display: 'inline',
              }}
            >
              没有更多了
            </Typography>
          </Stack>
        )}
      </InfiniteScroll>
    </>
  );
}
