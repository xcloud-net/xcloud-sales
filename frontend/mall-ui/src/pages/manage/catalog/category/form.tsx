import u from '@/utils';
import http from '@/utils/http';
import {
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Spin, TreeSelect
} from 'antd';
import { useEffect, useState } from 'react';

export default (props: any) => {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);
  const [tree, _tree] = useState<any[]>([]);
  const [expandKeys, _expandKeys] = useState<any[]>([]);

  const [form] = Form.useForm();

  const queryTree = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/category/tree', {})
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          var data = [
            {
              title: '顶级分类',
              key: '0',
            },
            ...(res.data.Data || []),
          ];
          _tree(u.convertTreeNodesForTreeSelect(data));
          _expandKeys(u.getTreeKeys(data));
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  const save = (row: any) => {
    _loading(true);

    console.log(row);
    if (row.ParentCategoryId == '') {
      row.ParentCategoryId = null;
    }
    row.ParentCategoryId = row.ParentCategoryId || '0';
    row.ParentCategoryId = parseInt(row.ParentCategoryId);

    http.apiRequest
      .post('/mall-admin/category/save', {
        ...row,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('保存成功');
          ok();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    form.resetFields();
    if (data) {
      if (data.ParentCategoryId != undefined) {
        data.ParentCategoryId = data.ParentCategoryId.toString();
      }
      form.setFieldsValue(data);
    }
  }, [data, tree]);

  useEffect(() => {
    if (show) {
      queryTree();
    }
  }, [show]);

  return (
    <>
      <Modal
        confirmLoading={loading}
        visible={show}
        onCancel={() => hide()}
        onOk={() => form.submit()}
      >
        <Spin spinning={loading}>
          <Form
            form={form}
            onFinish={(e) => save(e)}
            labelCol={{ flex: '110px' }}
            labelAlign="right"
            wrapperCol={{ flex: 1 }}
          >
            <Form.Item name="Id">
              <Input type="hidden" />
            </Form.Item>
            <Form.Item label="父级" name="ParentCategoryId">
              <TreeSelect
                placeholder="无父级分类则视为顶级分类"
                treeData={tree}
                treeExpandedKeys={expandKeys}
                onTreeExpand={(e) => _expandKeys(e)}
                allowClear
              />
            </Form.Item>
            <Form.Item
              label="名称"
              name="Name"
              rules={[{ required: true }, { max: 10 }]}
            >
              <Input />
            </Form.Item>
            <Form.Item label="SeoName" name="SeoName" rules={[{ max: 20 }]}>
              <Input />
            </Form.Item>
            <Form.Item label="描述" name="Description" rules={[{ max: 50 }]}>
              <Input.TextArea />
            </Form.Item>
            <Form.Item label="排序" name="DisplayOrder">
              <InputNumber defaultValue={0} />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};
