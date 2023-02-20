import { ErrorBlock } from 'antd-mobile';

export default () => {
  return (
    <>
      <div
        style={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
          paddingTop: 100,
        }}
      >
        <ErrorBlock status="empty" />
      </div>
    </>
  );
};
