import logoSvg from '@/assets/logo-no-background.png';
import XFooter from '@/components/footer';
import { PageContainer, ProLayout } from '@ant-design/pro-layout';
import { Col, Layout, Row, Spin } from 'antd';
import { history, Link, useLocation, useModel } from 'umi';
import XMenu from '../menu/data';
import appBox from './appBox';
import XLoginGuide from './loginGuide';
import XTopRight from './topRight';

const index = (props: any) => {
  const { children } = props;

  const storeAppAccountModel = useModel('storeAppAccount');

  const location = useLocation();

  if (!storeAppAccountModel.StoreAdminLoaded) {
    return (
      <Row
        justify="center"
        style={{
          marginTop: 300,
        }}
      >
        <Col span={1}>
          <Spin spinning size="large" />
        </Col>
      </Row>
    );
  }

  if (!storeAppAccountModel.isAdminLogin()) {
    return <XLoginGuide />;
  }

  return (
    <>
      <ProLayout
        //style={{ backgroundColor: 'white' }}
        appList={appBox}
        location={location}
        navTheme="light"
        layout="top"
        //fixedHeader
        splitMenus
        logo={
          <img
            style={{
              width: '100%',
            }}
            src={logoSvg}
            onClick={() => {
              history.push('/manage');
            }}
          />
        }
        title={false}
        menu={{ request: async () => XMenu }}
        menuItemRender={(x: any, dom: any) => {
          return <Link to={x.path}>{dom}</Link>;
        }}
        rightContentRender={() => <XTopRight />}
        footerRender={() => (
          <Layout.Footer>
            <XFooter />
          </Layout.Footer>
        )}
        waterMarkProps={{
          content:
            storeAppAccountModel.StoreAdmin.SysUser?.IdentityName || '--',
        }}
      >
        <div
          style={{
            minHeight: 500,
          }}
        >
          <PageContainer>{children}</PageContainer>
        </div>
      </ProLayout>
    </>
  );
};

export default index;
