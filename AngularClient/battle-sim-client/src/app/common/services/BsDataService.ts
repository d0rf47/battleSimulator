import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { AjaxHttpClientAdapter } from  'breeze-bridge2-angular';
import {
    EntityManager,
    EntityQuery,
    DataService,
    ValidationOptions,
    SaveOptions,
    core,
    EntityState,
    DataType,
    config
} from 'breeze-client';
import { DataServiceWebApiAdapter } from 'breeze-client/adapter-data-service-webapi';
import { HttpClient } from "@angular/common/http";
import { Title } from "@angular/platform-browser";

@Injectable()
export class BsDataService {
    private serviceName = [''];
    private managers: Record<string, any> = {};

    constructor(
        private readonly router: Router,
        private readonly http: HttpClient,
        private readonly titleService: Title        
    ){
        this.serviceName = [''];
        this.managers = {};
        config.registerAdapter('ajax', <any>function() {
            return new AjaxHttpClientAdapter(http);
        });
        config.initializeAdapterInstance('ajax', AjaxHttpClientAdapter.adapterName, true);
        DataServiceWebApiAdapter.register();
    }

    manager(key: string): any {
        let actualKey: string = key ?? 'default';

        if (!Object.prototype.hasOwnProperty.call(this.managers, actualKey)) {
            this.managers[actualKey] = {};
        }

        return this.managers[actualKey];
    }

    setServicePath(path: any, key:any):void {
        
        this.serviceName = path;
        const dsConfig = {
            serviceName: this.serviceName.toString(),
            hasServerMetadata: false
        };
        const dataService = new DataService(dsConfig);
        const validationOptions = new ValidationOptions ({
            validateOnAttach: true,
            validateOnSave: false,
            validateOnQuery: true
        });
        
        if(typeof key === 'undefined' || key === null) {
            key = 'default';
        }

        this.managers[key] = new EntityManager({dataService: dataService, validationOptions: validationOptions});
        this.managers[key].key = key;
    }
}