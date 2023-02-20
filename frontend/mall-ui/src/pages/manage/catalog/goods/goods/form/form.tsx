import u from '@/utils';
import http from '@/utils/http';
import {
  Alert,
  Button,
  Card,
  Col,
  Form,
  Input,
  InputNumber,
  Row,
  Select,
  Switch,
  TreeSelect,
  message,
} from 'antd';
import pinyin from 'pinyin';
import { useEffect, useState } from 'react';
// import style manually
import XMDEditor from '@/components/markdown/editor';
import React from 'react';

export default (props: any) => {
  const { data, ok, show } = props;
  const [loading, _loading] = useState(false);

  const [categoryTree, _categoryTree] = useState<any[]>([]);
  const [brandList, _brandList] = useState<any[]>([]);

  const [expandKeys, _expandKeys] = useState<any[]>([]);

  const [md, _md] = useState('');

  const [form] = Form.useForm();
  const formEnter = React.useRef<any>();

  useEffect(() => {
    setTimeout(() => {
      show && formEnter.current?.focus();
    }, 100);
  }, [show]);

  const convertPinyin = (name: any) => {
    var pyArr = pinyin(name || '', {
      style: pinyin.STYLE_FIRST_LETTER,
    });
    var py: any = u.flatMap(pyArr, (x) => x);
    py = py.join('').toLowerCase();
    return py;
  };

  const queryCategoryTree = () => {
    http.apiRequest
      .post('/mall-admin/category/tree')
      .then((res) => {
        var nodes = res.data.Data || [];
        u.findTreeNodes(nodes, (x: any) => {
          x.title = `${x.title}--${convertPinyin(x.title)}`;
        });
        var treeData = [
          {
            key: '0',
            title: '无分类',
          },
          ...nodes,
        ];
        _categoryTree(u.convertTreeNodesForTreeSelect(treeData));
        _expandKeys(u.getTreeKeys(treeData));
      })
      .finally(() => {});
  };

  const queryBrandList = () => {
    http.apiRequest
      .post('/mall-admin/brand/list', {})
      .then((res) => {
        var brands = res.data.Data || [];

        brands = u.map(brands, (x) => {
          return {
            ...x,
            py: convertPinyin(x.Name),
          };
        });

        _brandList([
          {
            Id: 0,
            Name: '无品牌',
          },
          ...brands,
        ]);
      })
      .finally(() => {});
  };

  const save = (row: any) => {
    _loading(true);

    console.log('raw form data', row);

    if (row.CategoryId) {
      row.CategoryId = parseInt(row.CategoryId);
    }

    if (row.BrandId) {
      row.BrandId = parseInt(row.BrandId);
    }

    http.apiRequest
      .post('/mall-admin/goods/save', {
        ...row,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          var goods = res.data.Data || {};
          ok && ok(goods);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryCategoryTree();
    queryBrandList();
  }, []);

  useEffect(() => {
    form.resetFields();
    if (data) {
      var formData = {
        ...data,
      };

      if (data.CategoryId) {
        formData.CategoryId = u.toString(data.CategoryId);
      }

      form.setFieldsValue(formData);
      //set markdown editor
      _md(formData.FullDescription || '');
    }
  }, [data]);

  return (
    <>
      <Card
        title="基础信息"
        size="small"
        loading={loading}
        style={{
          marginBottom: 10,
        }}
        extra={
          <Button
            type="primary"
            size="small"
            onClick={() => {
              form.submit();
            }}
          >
            保存
          </Button>
        }
      >
        <Form
          form={form}
          onFinish={(e) => save(e)}
          labelCol={{ flex: '110px' }}
          labelAlign="right"
          wrapperCol={{ flex: 1 }}
        >
          <Form.Item name="Id" hidden>
            <InputNumber type="hidden" />
          </Form.Item>
          <Row gutter={10}>
            <Col span={8}>
              <Form.Item
                label="名称"
                name="Name"
                rules={[
                  {
                    required: true,
                  },
                  {
                    max: 20,
                  },
                ]}
              >
                <Input
                  ref={formEnter}
                  placeholder="输入商品名称，比如【膳魔师保温杯】"
                />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item label="品牌" name="BrandId">
                <Select
                  placeholder="输入拼音缩写可以搜索..."
                  allowClear
                  showSearch
                  filterOption={(input, option) => {
                    var name = option?.children as unknown as string;
                    var match =
                      input &&
                      name &&
                      name.toLowerCase().indexOf(input.toLowerCase()) >= 0;
                    return match as boolean;
                  }}
                >
                  {brandList.map((item: any) => {
                    return (
                      <Select.Option key={item.Id} value={item.Id}>
                        {`${item.Name}--${item.py || 'N/A'}`}
                      </Select.Option>
                    );
                  })}
                </Select>
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item label="分类" name="CategoryId">
                <TreeSelect
                  key="key"
                  placeholder="输入拼音缩写可以搜索..."
                  treeData={categoryTree}
                  treeExpandedKeys={expandKeys}
                  onTreeExpand={(e) => {
                    _expandKeys(e);
                  }}
                  allowClear
                  showSearch
                  showArrow
                  treeNodeFilterProp="title"
                  onSearch={(e) => {
                    //
                  }}
                />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={10}>
            <Col span={8}>
              <Form.Item label="售价" name="Price">
                <InputNumber />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item label="成本价" name="CostPrice">
                <InputNumber />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item label="库存" name="StockQuantity">
                <InputNumber />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={10}>
            <Col span={12}>
              <Form.Item
                label="简短介绍"
                name="ShortDescription"
                rules={[{ max: 50 }]}
              >
                <Input.TextArea placeholder="将在商品详情页显示" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="店家评论"
                name="AdminComment"
                rules={[{ max: 50 }]}
              >
                <Input.TextArea placeholder="可以显示在搜索结果中" />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={10}>
            <Col span={6}>
              <Form.Item
                label="置顶展示"
                valuePropName="checked"
                name="StickyTop"
              >
                <Switch />
              </Form.Item>
            </Col>
            <Col span={6}>
              <Form.Item label="新品" valuePropName="checked" name="IsNew">
                <Switch />
              </Form.Item>
            </Col>
            <Col span={6}>
              <Form.Item label="热销" valuePropName="checked" name="IsHot">
                <Switch />
              </Form.Item>
            </Col>
            <Col span={6}>
              <Form.Item label="在售" valuePropName="checked" name="Published">
                <Switch />
              </Form.Item>
            </Col>
          </Row>
          <Form.Item
            label="商品详情"
            name="FullDescription"
            rules={[{ max: 10000 }]}
          >
            <Input.TextArea hidden style={{ display: 'none' }} />
            <Alert
              message="使用markdown语法"
              style={{
                marginBottom: 10,
              }}
            ></Alert>
            <XMDEditor
              value={md}
              onChange={(text: any) => {
                form.setFieldsValue({ FullDescription: text });
              }}
            />
          </Form.Item>
        </Form>
      </Card>
    </>
  );
};
